using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Application.User.UseCases.Login;
using NetWorthTracker.Application.User.UseCases.Register;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Username, request.Password));

        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpPost("/register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterCommand(request.Username, request.Password, request.Email));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.FirstOrDefault());
    }
}