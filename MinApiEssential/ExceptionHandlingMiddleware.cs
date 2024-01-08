namespace MinApiEssential;

using Users;
using System.Collections.Immutable;
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        try
        {
            await next(context);
        }
        catch (EmailIncorrectException e)
        {
            logger.LogWarning("Email incorrect '{Email}'", e.Mail);

            await Results.ValidationProblem(
                    errors: ImmutableDictionary<string, string[]>.Empty.Add("Email", [ "Email is invalid" ]),
                    statusCode: StatusCodes.Status422UnprocessableEntity)
                .ExecuteAsync(context);
        }
        catch (BadHttpRequestException e)
        {
            logger.LogError("Bad Request: {Error}", e);

            await Results.BadRequest(e).ExecuteAsync(context);
        }
        catch (Exception e)
        {
            logger.LogError("Undefined error: {Error}", e);

            await Results.StatusCode(StatusCodes.Status500InternalServerError).ExecuteAsync(context);
        }
    }
}
