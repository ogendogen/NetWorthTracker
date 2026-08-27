using MediatR;
using NetWorthTracker.Application.Common;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        //todo : validate command

        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(
            request.Username,
            request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            return Result<RegisterResponse>.Failure(
                new Error("UserAlreadyExists", "User with provided credentials already exists."));
        }

        var success =
            await _userRepository.RegisterAsync(request.Username, request.Password, request.Email, cancellationToken);

        return success
            ? Result<RegisterResponse>.Success(new RegisterResponse(true))
            : Result<RegisterResponse>.Failure(new Error("RegistrationFailed", "User registration failed."));
    }
}