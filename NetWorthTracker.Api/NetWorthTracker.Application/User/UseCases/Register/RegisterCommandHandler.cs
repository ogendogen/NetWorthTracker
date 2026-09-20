using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Domain.Common.Interfaces;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Application.User.UseCases.Register;

public class RegisterCommandHandler : BaseRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IServiceProvider? services = null,
        ILogger<RegisterCommandHandler>? logger = null)
        : base(services, logger!)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    protected override async ValueTask<Result<RegisterResponse>> Handler(
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

        var success = await _userRepository.RegisterAsync(request.Username, request.Password, request.Email, cancellationToken);

        if (success)
        {
            _emailService.SendPostRegistrationEmail(request.Username, request.Email);
        }

        //todo : log failures
        return success
            ? Result.Ok(new RegisterResponse(true))
            : Result.Fail<RegisterResponse>("User registration failed.");
    }
}