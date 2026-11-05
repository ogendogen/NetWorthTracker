using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class ResultRequestHandler<TRequest>
    : RequestHandlerBase<TRequest>, IRequestHandler<TRequest, Result>
    where TRequest : IRequest<Result>
{
    protected ResultRequestHandler(IServiceProvider? services, ILogger<ResultRequestHandler<TRequest>> logger)
        : base(services, logger)
    {
    }

    public async ValueTask<Result> Handle(TRequest request, CancellationToken cancellationToken)
    {
        LogHandling(request);
        var errors = await ValidateRequest(request, cancellationToken);

        if (errors.Length == 0)
        {
            return await Handler(request, cancellationToken);
        }

        return Result.Fail("Validation failed").WithErrors(errors);
    }

    protected abstract ValueTask<Result> Handler(TRequest request, CancellationToken cancellationToken);
}
