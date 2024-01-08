#pragma warning disable CS1591// Missing XML comment for publicly visible type or member
namespace MinApiEssential.Extensions;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
public static class OpenApiExtension
{
    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        int statusCode = StatusCodes.Status200OK,
        string? description = null,
        string? example = null,
        string? contentType = null,
        params string[] additionalContentTypes)
    {
        return builder
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
    }
    
    private static void SetExample(this OpenApiResponse response, string example)
    {
        response.Content.Values.First().Example = new OpenApiString(example);
    }
}
