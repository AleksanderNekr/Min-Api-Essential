namespace MinApiEssential.Users;

using Microsoft.AspNetCore.Mvc;
internal static class UserEndpoints
{
    public static void MapUserApi(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/user")
            .WithTags("Users API");

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

    private static UserResponse GetUser([FromRoute] Guid id)
    {
        return new UserResponse(id, "Alex", "alex@m.ru");
    }

    private static IResult PostUser([FromBody] UserRequest userRequest,
                                    [FromServices] ILogger<Program> logger)
    {
        UserResponse created = new(Guid.NewGuid(), userRequest.Name, userRequest.Email);
        logger.LogInformation("Created user: {Created}", created);

        return Results.Ok(created.Id);
    }
}
