using FluentResults;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class ValidatedHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    private readonly IServiceProvider? _services;

    protected ValidatedHandler(IServiceProvider? services)
    {
        _services = services;
    }

    public async ValueTask<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var validators = _services?.GetServices<IValidator<TRequest>>() ?? [];

        await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAndThrowAsync(request, cancellationToken)));

        return await Handler(request, cancellationToken);
    }

    protected abstract ValueTask<Result<TResponse>> Handler(
        TRequest request,
        CancellationToken cancellationToken);
}