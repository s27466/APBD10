namespace WebApplication1.Middleware;


public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
            throw;
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Console.WriteLine("Middleware is working. Logging error to file.");
    }
}


