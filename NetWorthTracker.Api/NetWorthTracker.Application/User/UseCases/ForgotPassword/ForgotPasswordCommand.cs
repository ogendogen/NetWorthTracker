using FluentResults;
using Mediator;

namespace NetWorthTracker.Application.User.UseCases.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
