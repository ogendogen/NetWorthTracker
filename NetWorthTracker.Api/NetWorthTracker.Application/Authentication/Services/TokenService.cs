using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NetWorthTracker.Application.Authentication.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public LoginResponse CreateLoginResponse(string userName)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.LifetimeMinutes);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(ClaimTypes.Name, userName)
            ],
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            userName);
    }

    public string GenerateEmailToken(string userEmail)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
            new Claim(ClaimTypes.Email, userEmail),
            new Claim("purpose", "email_confirmation")
            ]),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Result<string?> ValidateEmailToken(string token, out string? email)
    {
        email = null;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidIssuer = _jwtSettings.Issuer,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(1)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var purposeClaim = principal.FindFirst("purpose")?.Value;
            if (purposeClaim != "email_confirmation")
            {
                _logger.LogWarning("Invalid email token purpose: {Purpose}", purposeClaim);
                return Result.Fail("Invalid email token purpose");
            }

            email = principal.FindFirst(ClaimTypes.Email)?.Value;
            return Result.Ok(principal.FindFirst(ClaimTypes.Email)?.Value);
        }
        catch (Exception)
        {
            _logger.LogWarning("Invalid email token: {Token}", token);
            return Result.Fail("Invalid email token");
        }
    }
}