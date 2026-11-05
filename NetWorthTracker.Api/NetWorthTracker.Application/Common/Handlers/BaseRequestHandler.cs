using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class BaseRequestHandler<TRequest, TResponse>
    : RequestHandlerBase<TRequest>, IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    protected BaseRequestHandler(IServiceProvider? services, ILogger<BaseRequestHandler<TRequest, TResponse>> logger)
        : base(services, logger)
    {
    }

    public async ValueTask<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        LogHandling(request);
        var errors = await ValidateRequest(request, cancellationToken);

        if (errors.Length == 0)
        {
            return await Handler(request, cancellationToken);
        }

        return Result.Fail<TResponse>("Validation failed").WithErrors(errors);
    }

    protected abstract ValueTask<Result<TResponse>> Handler(
        TRequest request,
        CancellationToken cancellationToken);
}
