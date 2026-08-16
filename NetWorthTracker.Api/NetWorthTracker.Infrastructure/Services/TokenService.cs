using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Application.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Infrastructure.Configurations;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NetWorthTracker.Infrastructure.Services;

public sealed class TokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

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
}