using FluentResults;
using FluentValidation;
using MediatR;
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

    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var validators = _services?.GetServices<IValidator<TRequest>>() ?? [];

        await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAndThrowAsync(request, cancellationToken)));

        return await Handler(request, cancellationToken);
    }

    protected abstract Task<Result<TResponse>> Handler(
        TRequest request,
        CancellationToken cancellationToken);
}