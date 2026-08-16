using MediatR;
using NetWorthTracker.Application.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Login;

public class LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await userRepository.LoginAsync(request.Username, request.Password, cancellationToken);

        return result
            ? tokenService.CreateLoginResponse(request
                .Username)
            : null;
    }
}