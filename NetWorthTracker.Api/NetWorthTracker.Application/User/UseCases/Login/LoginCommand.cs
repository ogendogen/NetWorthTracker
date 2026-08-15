using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Application.User.UseCases.Login;

public record LoginCommand(string Name, string Password) : IRequest<Result<LoginCommandHandler>>;
