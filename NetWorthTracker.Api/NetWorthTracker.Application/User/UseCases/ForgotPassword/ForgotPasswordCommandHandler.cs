using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.ForgotPassword;
using NetWorthTracker.Domain.Common.Interfaces;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.ForgotPassword;

public class ForgotPasswordCommandHandler : BaseRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        ITokenService tokenService,
        IServiceProvider? services = null,
        ILogger<ForgotPasswordCommandHandler>? logger = null)
        : base(services, logger!)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    // Always succeeds so the response cannot reveal whether the email is registered.
    protected override async ValueTask<Result<ForgotPasswordResponse>> Handler(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is { IsEmailConfirmed: true })
        {
            var resetToken = _tokenService.GeneratePasswordResetToken(user.Email);
            var encodedToken = Uri.EscapeDataString(resetToken);
            _emailService.SendPasswordResetEmail(user.Login, user.Email, encodedToken);
        }

        return Result.Ok(new ForgotPasswordResponse(true));
    }
}
