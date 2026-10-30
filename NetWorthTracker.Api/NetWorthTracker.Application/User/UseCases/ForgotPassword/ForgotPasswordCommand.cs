using FluentResults;
using Mediator;
using NetWorthTracker.Application.User.Models.ForgotPassword;

namespace NetWorthTracker.Application.User.UseCases.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResponse>>;
