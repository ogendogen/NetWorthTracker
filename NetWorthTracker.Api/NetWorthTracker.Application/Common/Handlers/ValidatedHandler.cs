using FluentResults;
using FluentValidation;
using MediatR;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class ValidatedHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    protected ValidatedHandler(IEnumerable<IValidator<TRequest>>? validators = null)
    {
        _validators = validators ?? [];
    }

    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _validators.Select(validator =>
                validator.ValidateAndThrowAsync(request, cancellationToken)));

        return await Handler(request, cancellationToken);
    }

    protected abstract Task<Result<TResponse>> Handler(
        TRequest request,
        CancellationToken cancellationToken);
}
