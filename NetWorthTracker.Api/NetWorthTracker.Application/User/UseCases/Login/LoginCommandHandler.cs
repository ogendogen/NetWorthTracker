using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Login;

public class LoginCommandHandler : BaseRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IServiceProvider? services = null,
        ILogger<LoginCommandHandler>? logger = null)
        : base(services, logger!)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    protected override async Task<Result<LoginResponse>> Handler(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _userRepository.LoginAsync(request.Username, request.Password, cancellationToken);

        if (!result)
        {
            return Result.Fail("User with provided credentials does not exist.");
        }

        var token = _tokenService.CreateLoginResponse(request.Username);

        return Result.Ok(new LoginResponse(token.AccessToken, token.ExpiresAt, token.UserName));
    }
}
