using MediatR;
using NetWorthTracker.Application.Exceptions;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse?>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RegisterResponse?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        //todo : validate command

        var existingUser = await _userRepository.GetByUsernameAsync(
            request.Username,
            request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException();
        }

        var success =
            await _userRepository.RegisterAsync(request.Username, request.Password, request.Email, cancellationToken);

        return new RegisterResponse(success);
    }
}