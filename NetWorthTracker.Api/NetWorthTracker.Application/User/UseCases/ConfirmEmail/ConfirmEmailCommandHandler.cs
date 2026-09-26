using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.ConfirmEmail;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler : BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public ConfirmEmailCommandHandler(
        IServiceProvider? services,
        ILogger<BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>> logger,
        ITokenService tokenService,
        IUserRepository userRepository)
        : base(services, logger)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    protected override async ValueTask<Result<ConfirmEmailResponse>> Handler(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = _tokenService.ValidateEmailToken(request.Token, out string email);

        if (result.IsFailed)
        {
            return await ValueTask.FromResult(Result.Fail<ConfirmEmailResponse>("Invalid token"));
        }

        var confirmationResult = await _userRepository.ConfirmEmailByEmail(email, cancellationToken);
        return await ValueTask.FromResult(Result.Ok(new ConfirmEmailResponse(IsEmailConfirmed: confirmationResult)));
    }
}
