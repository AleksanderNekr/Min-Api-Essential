using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        var auth = app.MapGroup("/auth")
            .WithTags("Auth");

        auth.MapPost("/register", Register)
            .WithSummary("Register User")
            .AddDescriptionFor(StatusCodes.Status200OK, "Sets cookie")
            .AddDescriptionFor(StatusCodes.Status400BadRequest, "If email is invalid, or password is too simple, or email is taken");

        auth.MapPost("/login", Login)
            .WithSummary("Login as a User")
            .AddDescriptionFor(StatusCodes.Status200OK, "Access token if no cookies requested")
            .Produces<ProblemDetails>(
                StatusCodes.Status401Unauthorized,
                contentType: MediaTypeNames.Application.ProblemJson,
                description: "If auth failed")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "If true – then cookies will be set, no access token returns";
                operation.Parameters[1].Description = "If true – cookies will be set for a session only";
                return operation;
            });

        auth.MapPost("/tokens", Refresh)
            .WithSummary("Refresh access token")
            .AddDescriptionFor(StatusCodes.Status200OK, "Access token")
            .Produces<UnauthorizedHttpResult>(
                StatusCodes.Status401Unauthorized,
                contentType: MediaTypeNames.Application.ProblemJson,
                description: "If auth failed");
    }

    private static async Task<Results<Ok, ValidationProblem>> Register(
        [FromBody] SignInRequest registration,
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

    private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(
        [FromBody] SignInRequest signIn,
        [AsParameters] CookiesRequest cookies,
        [FromServices] SignInManager<User> signInManager)
    {
        var useCookieScheme = cookies.useCookies is true || cookies.useSessionCookies is true;
        var isPersistent = cookies is { useCookies: true, useSessionCookies: not true };
        signInManager.AuthenticationScheme = useCookieScheme
            ? IdentityConstants.ApplicationScheme
            : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, isPersistent, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Empty;
    }

    private static async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh(
        [FromBody] RefreshRequest refreshRequest,
        [FromServices] SignInManager<User> signInManager,
        [FromServices] IServiceProvider serviceProvider)
    {
        var bearerTokenOptions = serviceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties.ExpiresUtc is not {} expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not {} user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    /// <summary>
    /// Request body
    /// </summary>
    /// <param name="Email">User's email address</param>
    /// <param name="Password">Password</param>
    private sealed record SignInRequest([property: EmailAddress] string Email, string Password);

    /// <summary>
    /// Cookies options
    /// </summary>
    /// <param name="useCookies">If true – then cookies will be set, no access token returns</param>
    /// <param name="useSessionCookies">If true – cookies will be set for only a session</param>
    private sealed record CookiesRequest(bool? useCookies, bool? useSessionCookies);

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