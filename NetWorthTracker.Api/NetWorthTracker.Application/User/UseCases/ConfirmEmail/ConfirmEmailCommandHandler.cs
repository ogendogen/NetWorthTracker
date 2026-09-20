using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.ConfirmEmail;

namespace NetWorthTracker.Application.User.UseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler : BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    private readonly ITokenService _tokenService;

    public ConfirmEmailCommandHandler(
        IServiceProvider? services,
        ILogger<BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>> logger,
        ITokenService tokenService)
        : base(services, logger)
    {
        _tokenService = tokenService;
    }

    protected override ValueTask<Result<ConfirmEmailResponse>> Handler(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = _tokenService.ValidateEmailToken(request.Token);

        if (result.IsFailed)
        {
            return ValueTask.FromResult(Result.Fail<ConfirmEmailResponse>("Invalid token"));
        }

        return ValueTask.FromResult(Result.Ok(new ConfirmEmailResponse(IsEmailConfirmed: true)));
    }
}
