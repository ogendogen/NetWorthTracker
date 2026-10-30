using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.ResetPassword;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.ResetPassword;

public class ResetPasswordCommandHandler : BaseRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public const string InvalidTokenError = "Invalid or expired password reset token";

    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IServiceProvider? services = null,
        ILogger<ResetPasswordCommandHandler>? logger = null)
        : base(services, logger!)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    protected override async ValueTask<Result<ResetPasswordResponse>> Handler(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var tokenResult = _tokenService.ValidatePasswordResetToken(request.Token, out var email);

        if (tokenResult.IsFailed || string.IsNullOrEmpty(email))
        {
            return Result.Fail<ResetPasswordResponse>(InvalidTokenError);
        }

        var updated = await _userRepository.UpdatePasswordByEmailAsync(email, request.Password, cancellationToken);

        return updated
            ? Result.Ok(new ResetPasswordResponse(true))
            : Result.Fail<ResetPasswordResponse>(InvalidTokenError);
    }
}
