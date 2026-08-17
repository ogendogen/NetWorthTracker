using MediatR;
using NetWorthTracker.Application.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.LoginAsync(request.Username, request.Password, cancellationToken);

        return result
            ? _tokenService.CreateLoginResponse(request
                .Username)
            : null;
    }
}