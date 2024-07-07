using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinApiEssential.Extensions;

namespace MinApiEssential.Users;

internal static class UserEndpoints
{
    public static void MapUserApi(this WebApplication app)
    {
        var group = app.MapGroup("/user")
            .WithTags("Users API")
            .RequireAuthorization(IdentityConstants.BearerScheme);

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
        return new UserResponse(id, "Alex", new MailAddress("alex@m.ru"));
    }

    private static IResult PostUser([FromBody] UserRequest userRequest,
                                    ILogger<Program> logger)
    {
        if (!MailAddress.TryCreate(userRequest.Email, out var email))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]> { { "email", [ "Incorrect email value" ] } },
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        UserResponse created = new(Guid.NewGuid(), userRequest.Name, email);
        logger.LogInformation("Created user: {Created}", created);

        return Results.Ok(created.Id);
    }
}