using MediatR;
using NetWorthTracker.Application.User.Models.Register;

namespace NetWorthTracker.Application.User.UseCases.Register;

public record RegisterCommand(string Username, string Password, string Email) : IRequest<RegisterResponse?>;