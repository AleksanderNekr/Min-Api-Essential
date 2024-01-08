#pragma warning disable CS1591// Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Mvc;
using MinApiEssential.Extensions;
using MinApiEssential.Filters;
using MinApiEssential.Models;
using System.Net.Mime;

namespace MinApiEssential;

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

        group.MapGet("/greet", () => "hello!")
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
    }

    private static void MapUserEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder userGroup = app.MapGroup("/user")
            .WithTags("Users API");

        userGroup.MapGet("/{id:guid}", (Guid id) => new User(id, "Alex", "alex@m.ru"))
            .WithSummary("Find a user by id")
            .Produces<User>(description: "Found User");

        userGroup.MapPost("/", (
                [FromBody] UserRequest userRequest,
                ILogger<Program> logger) =>
            {
                User created = new(Guid.NewGuid(), userRequest.Name, userRequest.Email);
                logger.LogInformation("Created user: {Created}", created);
                return created.Id.ToString();
            })
            .WithSummary("Create User")
            .Produces<string>(description: "ID of the created user", example: Guid.NewGuid().ToString())
            .Produces<ProblemDetails>(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                description: "If the request body model is invalid")
            .AddEndpointFilter<UserRequestValidationFilter>();
    }
}
