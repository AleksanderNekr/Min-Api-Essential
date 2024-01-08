#pragma warning disable CS1591// Missing XML comment for publicly visible type or member
namespace MinApiEssential;

using Microsoft.AspNetCore.Mvc;
using Extensions;
using Models;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Immutable;
using System.Net.Mail;
public static class EndpointsMapper
{
    public static void MapMinimalEndpoints(this WebApplication app)
    {
        MapTestEndpoints(app);
        MapUserEndpoints(app);
    }

    private static void MapTestEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/test")
            .WithTags("Test endpoints group");

        group.MapGet("/greet", GetGreet)
            .WithSummary("Test API endpoint")
            .WithDescription("Test endpoint")
            .Produces<string>(
                contentType: MediaTypeNames.Text.Plain,
                description: "Successfully fetched data",
                example: "hello!")
            .Produces<string>(
                StatusCodes.Status500InternalServerError,
                contentType: MediaTypeNames.Text.Plain,
                description: "Server is not responding",
                example: "Internal server error, try again later");
        return;

        string GetGreet()
        {
            return "hello!";
        }
    }

    private static void MapUserEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder userGroup = app.MapGroup("/user")
            .WithTags("Users API");

        userGroup.MapGet("/{id:guid}", GetUser)
            .WithSummary("Find a user by id")
            .Produces<User>(description: "Found User");

        userGroup.MapPost("/", PostUser)
            .WithSummary("Create User")
            .Produces<Guid>(description: "ID of the created user")
            .Produces<ProblemDetails>(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                description: "If the request body model is invalid");
        return;

        IResult PostUser([FromBody] UserRequest userRequest,
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

        User GetUser([FromRoute] Guid id)
        {
            return new User(id, "Alex", "alex@m.ru");
        }

        IResult ValidateUserRequest(UserRequest userRequest)
        {
            if (!MailAddress.TryCreate(userRequest.Email, out _))
            {
                return Results.ValidationProblem(
                    errors: ImmutableDictionary<string, string[]>.Empty.Add("Email", [ "Email is invalid" ]),
                    statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            return Results.Ok();
        }
    }
}
