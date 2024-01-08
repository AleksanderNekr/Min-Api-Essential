namespace MinApiEssential.Test;

using System.Net.Mime;
internal static class TestEndpoints
{
    public static void MapTestApi(this WebApplication app)
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
    }

    private static string GetGreet()
    {
        return "hello!";
    }
}
