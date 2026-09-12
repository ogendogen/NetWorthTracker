using System.Diagnostics;

namespace NetWorthTracker.Api.Middleware;

public sealed class HttpRequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<HttpRequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();

        try
        {
            await next(context);

            logger.LogInformation(
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds:F3} ms",
                context.Request.Method,
                context.Request.Path.Value ?? "/",
                context.Response.StatusCode,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds:F3} ms",
                context.Request.Method,
                context.Request.Path.Value ?? "/",
                StatusCodes.Status500InternalServerError,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);

            throw;
        }
    }
}
