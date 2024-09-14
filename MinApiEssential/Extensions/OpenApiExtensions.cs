using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MinApiEssential.Extensions;

internal static class OpenApiExtensions
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

    public static TBuilder WithTag<TBuilder>(this TBuilder builder, string name, string description)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.WithTags(name);
        AddTagDescriptionsDocumentFilter.AddTag(new OpenApiTag { Name = name, Description = description });
        return builder;
    }

    public static void AddTagDescriptionHandler(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.DocumentFilter<AddTagDescriptionsDocumentFilter>();
    }

    private static void SetExample(this OpenApiResponse response, string example)
    {
        response.Content.Values.First().Example = new OpenApiString(example);
    }
}

file sealed class AddTagDescriptionsDocumentFilter : IDocumentFilter
{
    private static readonly List<OpenApiTag> MinimalApiTags = [ ];

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags ??= new List<OpenApiTag>();

        foreach (var tag in MinimalApiTags)
        {
            swaggerDoc.Tags.Add(tag);
        }
    }

    public static void AddTag(OpenApiTag openApiTag)
    {
        MinimalApiTags.Add(openApiTag);
    }
}