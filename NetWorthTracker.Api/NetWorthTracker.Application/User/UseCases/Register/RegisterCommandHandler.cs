using FluentResults;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Register;

public class RegisterCommandHandler : ValidatedHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IServiceProvider? services = null)
        : base(services)
    {
        _userRepository = userRepository;
    }

    protected override async Task<Result<RegisterResponse>> Handler(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(
            request.Username,
            request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            return Result.Fail<RegisterResponse>(
                "User with provided credentials already exists.");
        }

        var success =
            await _userRepository.RegisterAsync(request.Username, request.Password, request.Email, cancellationToken);

        //todo : log failures
        return success
            ? Result.Ok(new RegisterResponse(true))
            : Result.Fail<RegisterResponse>("User registration failed.");
    }
}