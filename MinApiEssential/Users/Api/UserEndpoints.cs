using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinApiEssential.Extensions;
using MinApiEssential.Resources;
using static MinApiEssential.Resources.Resources;
using static MinApiEssential.Users.Api.Extensions;

namespace MinApiEssential.Users.Api;

internal static class UserEndpoints
{
    public static void MapUserApi(this WebApplication app)
    {
        var group = app.MapGroup("/user")
            .WithTags("Users API")
            .RequireAuthorization(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme);

        group.MapGet("/{id:guid}", GetUser)
            .WithSummary("Find a user by id")
            .Produces<UserResponse>(description: "Found User");

        group.MapPost("/", PostUser)
            .WithSummary("Create User")
            .Produces<Guid>(description: "ID of the created user")
            .Produces<HttpValidationProblemDetails>(
                StatusCodes.Status422UnprocessableEntity,
                description: "If the request body model is invalid");
    }

    private static UserResponse GetUser(Guid id)
    {
        return new UserResponse(id, "Alex", "alex@m.ru");
    }

    private static IResult PostUser([FromBody] UserRequest userRequest,
                                    ILogger<Program> logger)
    {
        if (!MailAddress.TryCreate(userRequest.Email, out var email))
        {
            return CreateValidationProblem(nameof(InvalidEmail), ErrorDescriber.FormatInvalidEmail(userRequest.Email));
        }

        UserResponse created = new(Guid.NewGuid(), userRequest.Name, email.Address);
        logger.LogInformation("Created user: {Created}", created);

        return Results.Ok(created.Id);
    }
}