using FluentResults;
using MediatR;
using NetWorthTracker.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Application.User.UseCases.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandHandler>>
{
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<Result<LoginCommandHandler>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = _userRepository.LoginAsync(request.Name, request.Password);
        return Task.FromResult(
            result == true
                ? Result.Ok(this)
                : Result.Fail<LoginCommandHandler>("Invalid username or password"));
    }
}
