using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace NetWorthTracker.Application.Common.Handlers;

public sealed class RequestLoggingBehavior<TMessage, TResponse>(
    ILogger<RequestLoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(message, cancellationToken);

        if (response is IResultBase result)
        {
            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Request {RequestType} completed successfully",
                    typeof(TMessage).Name);
            }
            else if (result.IsFailed)
            {
                logger.LogError(
                    "Request {RequestType} failed with errors {Errors}",
                    typeof(TMessage).Name,
                    string.Join(", ", result.Errors.Select(e => e.Message)));
            }
        }

        return response;
    }
}
