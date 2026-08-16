using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Application.User.UseCases.Login;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("/login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await mediator.Send(new LoginCommand(request.Username, request.Password));

        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok();
    }
}