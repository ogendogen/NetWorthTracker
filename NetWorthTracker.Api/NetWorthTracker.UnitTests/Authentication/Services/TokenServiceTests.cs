using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Application.Authentication;
using NetWorthTracker.Application.Authentication.Services;

namespace NetWorthTracker.Tests.Authentication.Services;

public class TokenServiceTests
{
    [Test]
    public async Task GivenValidJwtSettings_WhenCreatingLoginResponse_ThenReturnsValidSignedToken()
    {
        // Arrange
        const string username = "test-user";
        const string issuer = "test-issuer";
        const string audience = "test-audience";
        const string signingKey = "test-signing-key-with-at-least-32-characters";
        const int lifetimeMinutes = 60;
        var jwtSettings = new JwtSettings
        {
            Issuer = issuer,
            Audience = audience,
            SigningKey = signingKey,
            LifetimeMinutes = lifetimeMinutes
        };
        var options = IOptions<JwtSettings>.Mock();
        options.Value.Returns(jwtSettings);
        var tokenService = new TokenService(options.Object);
        var earliestExpiry = DateTimeOffset.UtcNow.AddMinutes(lifetimeMinutes);

        // Act
        var response = tokenService.CreateLoginResponse(username);
        var latestExpiry = DateTimeOffset.UtcNow.AddMinutes(lifetimeMinutes);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name
        };
        var principal = tokenHandler.ValidateToken(response.AccessToken, validationParameters, out var validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;

        await Assert.That(response.UserName).IsEqualTo(username);
        await Assert.That(response.ExpiresAt >= earliestExpiry).IsTrue();
        await Assert.That(response.ExpiresAt <= latestExpiry).IsTrue();
        await Assert.That(jwtToken.Header.Alg).IsEqualTo(SecurityAlgorithms.HmacSha256);
        await Assert.That(jwtToken.Issuer).IsEqualTo(issuer);
        await Assert.That(jwtToken.Audiences).Contains(audience);
        await Assert.That(jwtToken.Subject).IsEqualTo(username);
        await Assert.That(principal.Identity?.Name).IsEqualTo(username);
    }
}