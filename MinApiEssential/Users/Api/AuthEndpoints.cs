using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MinApiEssential.Extensions;
using static MinApiEssential.Users.Api.Extensions;

namespace MinApiEssential.Users.Api;

/// <summary>
/// Based on <see cref="Microsoft.AspNetCore.Routing.IdentityApiEndpointRouteBuilderExtensions"/>
/// </summary>
internal static class AuthEndpoints
{
    public static void MapAuthApi(this WebApplication app)
    {
        var auth = app.MapGroup("/auth")
            .WithTag("Auth", "Auth endpoints");

        auth.MapPost("/register", Register);

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

    /// <summary>
    /// Registers an user
    /// </summary>
    /// <param name="registration">Auth data request model</param>
    /// <param name="userManager">User manager service</param>
    /// <param name="userStore">User store service</param>
    /// <response code="200">Success, adds ability to log in</response>
    /// <response code="400">Auth data provided is unacceptable</response>
    /// <exception cref="NotSupportedException">Internal app settings error</exception>
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

        if (string.IsNullOrEmpty(email) || !MailAddress.TryCreate(email, out _))
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
        if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)

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
    private sealed record SignInRequest([property: EmailAddress] [property: DefaultValue("alex@mail.ru")] string Email, string Password);

    /// <summary>
    /// Cookies options
    /// </summary>
    /// <param name="useCookies">If true – then cookies will be set, no access token returns</param>
    /// <param name="useSessionCookies">If true – cookies will be set for only a session</param>
    private sealed record CookiesRequest(bool? useCookies, bool? useSessionCookies);
}