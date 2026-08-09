using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Api.Models;
using NetWorthTracker.Api.Services;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class AuthController(TokenService tokenService) : ControllerBase
{
    [HttpPost("/login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        if (request.Username != "test" || request.Password != "test")
        {
            return Unauthorized();
        }

        return Ok(tokenService.CreateLoginResponse(request.Username));
    }

    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok();
    }
}