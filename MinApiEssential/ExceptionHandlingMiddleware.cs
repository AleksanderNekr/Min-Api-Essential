namespace MinApiEssential;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException e)
        {
            await Results.BadRequest(e).ExecuteAsync(context);
        }
        catch
        {
            await Results.StatusCode(StatusCodes.Status500InternalServerError).ExecuteAsync(context);
            throw;
        }
    }
}