using FluentResults;
using Mediator;
using NetWorthTracker.Application.User.Models.ResetPassword;

namespace NetWorthTracker.Application.User.UseCases.ResetPassword;

public record ResetPasswordCommand(string Token, string Password, string ConfirmPassword)
    : IRequest<Result<ResetPasswordResponse>>;
