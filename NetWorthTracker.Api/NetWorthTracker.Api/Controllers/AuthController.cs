using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.User.Models.ConfirmEmail;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Application.User.UseCases.ConfirmEmail;
using NetWorthTracker.Application.User.UseCases.Login;
using NetWorthTracker.Application.User.UseCases.Register;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AuthController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("/login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Username, request.Password));

        return result.IsSuccess ? Ok(result.Value) : Unauthorized();
    }

    [HttpPost("/register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterCommand(request.Username, request.Password, request.Email));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("/confirm-email")]
    [ProducesResponseType<ConfirmEmailResponse>(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConfirmEmailResponse>> ConfirmEmail([FromQuery] string token)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand(token));

        return result.IsSuccess && result.Value.IsEmailConfirmed ? Redirect($"{_configuration["FrontendUrl"]}/email-confirmed") : Redirect($"{_configuration["FrontendUrl"]}/email-confirmation-failed");
    }
}
