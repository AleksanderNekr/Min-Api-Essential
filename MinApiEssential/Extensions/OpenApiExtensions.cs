using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MinApiEssential.Extensions;

public static class OpenApiExtensions
{
    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        int statusCode = StatusCodes.Status200OK,
        string? description = null,
        string? example = null,
        string? contentType = null,
        params string[] additionalContentTypes)
        => builder
            .Produces<TResponse>(statusCode, contentType, additionalContentTypes)
            .WithOpenApi(operation =>
            {
                if (description is not null)
                {
                    operation.Responses[$"{statusCode}"].Description = description;
                }

                if (example is not null)
                {
                    operation.Responses[$"{statusCode}"].SetExample(example);
                }

                return operation;
            });

    public static RouteHandlerBuilder AddDescriptionFor(
        this RouteHandlerBuilder builder,
        int statusCode,
        string? description)
        => builder.WithOpenApi(operation =>
        {
            operation.Responses[$"{statusCode}"].Description = description;

            return operation;
        });


    private static void SetExample(this OpenApiResponse response, string example)
    {
        response.Content.Values.First().Example = new OpenApiString(example);
    }
}