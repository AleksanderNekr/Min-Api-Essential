using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MinApiEssential.Extensions;

namespace MinApiEssential.Users;

/// <summary>
/// Based on <see cref="Microsoft.AspNetCore.Routing.IdentityApiEndpointRouteBuilderExtensions"/>
/// </summary>
internal static class AuthEndpoints
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public static void MapAuthApi(this WebApplication app)
    {
        RouteGroupBuilder authGroup = app.MapGroup("/auth")
            .WithTags("Auth & Identity");

        authGroup.MapPost("/register", Register)
            .WithSummary("Register User")
            .AddDescriptionFor(StatusCodes.Status200OK, "Sets cookie")
            .AddDescriptionFor(StatusCodes.Status400BadRequest, "If email is invalid, or password is too simple, or email is taken");
    }

    private static async Task<Results<Ok, ValidationProblem>> Register(
        [FromBody] RegisterRequest registration,
        HttpContext context,
        [FromServices] UserManager<User> userManager,
        [FromServices] IUserStore<User> userStore)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(MapAuthApi)} requires a user store with email support.");
        }

        var emailStore = (IUserEmailStore<User>)userStore;
        var email = registration.Email;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new User();
        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    #region Utils

    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription)
        => TypedResults.ValidationProblem(new Dictionary<string, string[]> { { errorCode, [ errorDescription ] } });

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [ error.Description ];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    #endregion
}