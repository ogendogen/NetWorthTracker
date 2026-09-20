using FluentResults;
using Mediator;
using NetWorthTracker.Application.User.Models.ConfirmEmail;

namespace NetWorthTracker.Application.User.UseCases.ConfirmEmail;

public record ConfirmEmailCommand(string Token) : IRequest<Result<ConfirmEmailResponse>>;
