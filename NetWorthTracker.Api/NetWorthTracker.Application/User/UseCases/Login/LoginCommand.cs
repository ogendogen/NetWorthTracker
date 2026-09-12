using FluentResults;
using Mediator;
using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.Application.User.UseCases.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;