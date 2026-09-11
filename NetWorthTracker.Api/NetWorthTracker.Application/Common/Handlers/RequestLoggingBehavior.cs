using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NetWorthTracker.Application.Common.Handlers;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        logger.LogInformation(
            "Completed MediatR request {RequestType}",
            typeof(TRequest).Name);

        if (response is IResultBase result && result.IsFailed)
        {
            logger.LogError(
                "Request {RequestType} failed with errors {Errors}",
                typeof(TRequest).Name,
                string.Join(", ", result.Errors.Select(e => e.Message)));
        }

        return response;
    }
}
