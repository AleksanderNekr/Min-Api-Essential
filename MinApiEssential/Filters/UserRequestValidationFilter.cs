namespace MinApiEssential.Filters;

using Models;
using System.Collections.Immutable;
using System.Net.Mail;
/// <inheritdoc />
public class UserRequestValidationFilter : IEndpointFilter
{
    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.GetArgument<UserRequest>(0);
        if (!MailAddress.TryCreate(user.Email, out _))
        {
            return Results.ValidationProblem(
                errors: ImmutableDictionary<string, string[]>.Empty.Add("Email", [ "Email is invalid" ]),
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return await next(context);
    }
}
