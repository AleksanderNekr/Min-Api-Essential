namespace MinApiEssential.Endpoints;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Immutable;
using System.Net.Mail;
internal static class UserEndpoints
{
    public static void MapUserApi(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/user")
            .WithTags("Users API");

        group.MapGet("/{id:guid}", GetUser)
            .WithSummary("Find a user by id")
            .Produces<User>(description: "Found User");

        group.MapPost("/", PostUser)
            .WithSummary("Create User")
            .Produces<Guid>(description: "ID of the created user")
            .Produces<ProblemDetails>(
                StatusCodes.Status422UnprocessableEntity,
                description: "If the request body model is invalid");
    }

    private static User GetUser([FromRoute] Guid id)
    {
        return new User(id, "Alex", "alex@m.ru");
    }

    private static IResult PostUser([FromBody] UserRequest userRequest,
                                    [FromServices] ILogger<Program> logger)
    {
        if (ValidateUserRequest(userRequest) is ProblemHttpResult problem)
        {
            logger.LogInformation("Generated problem {@Problem}", problem);
            return problem;
        }

        User created = new(Guid.NewGuid(), userRequest.Name, userRequest.Email);
        logger.LogInformation("Created user: {Created}", created);

        return Results.Ok(created.Id);
    }

    private static IResult ValidateUserRequest(UserRequest userRequest)
    {
        if (!MailAddress.TryCreate(userRequest.Email, out _))
        {
            return Results.ValidationProblem(
                ImmutableDictionary<string, string[]>.Empty.Add("Email", [ "Email is invalid" ]),
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Results.Ok();
    }
}
