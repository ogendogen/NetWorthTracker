using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Api.Models;
using NetWorthTracker.Api.Services;
using NetWorthTracker.Application.User.UseCases.Login;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TokenService _tokenService;

    public AuthController(IMediator mediator, TokenService tokenService)
    {
        _mediator = mediator;
    }

    [HttpPost("/login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Username, request.Password));

        if (result.IsFailed)
        {
            return Unauthorized(result.Errors);
        }

        return Ok(_tokenService.CreateLoginResponse(request.Username)); // todo: schowac ten serwis gdzies na poziomie aplikacji np. w handlerze
    }

    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok();
    }
}