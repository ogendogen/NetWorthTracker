using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Api.Services;

namespace NetWorthTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IExceptionResponseMapper exceptionResponseMapper)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = exceptionResponseMapper.Map(exception);

            logger.LogError(
                exception,
                "An unhandled exception occurred while processing {RequestMethod} {RequestPath}. Responding with {StatusCode}.",
                context.Request.Method,
                context.Request.Path,
                response.StatusCode);

            context.Response.Clear();
            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/problem+json";

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new ProblemDetails
                {
                    Status = response.StatusCode,
                    Title = response.Title,
                    Detail = response.Detail
                },
                JsonSerializerOptions.Web,
                context.RequestAborted);
        }
    }
}
