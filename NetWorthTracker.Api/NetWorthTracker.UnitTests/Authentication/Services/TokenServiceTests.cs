using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
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
        var logger = ILogger<TokenService>.Mock();
        var tokenService = new TokenService(options.Object, logger.Object);
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

    [Test]
    public async Task GivenEmail_WhenGeneratingPasswordResetToken_ThenTokenHasResetPurposeAndOneHourLifetime()
    {
        // Arrange
        const string email = "test-user@example.com";
        var tokenService = CreateTokenService();
        var earliestExpiry = DateTime.UtcNow.AddHours(1).AddSeconds(-5);

        // Act
        var token = tokenService.GeneratePasswordResetToken(email);
        var latestExpiry = DateTime.UtcNow.AddHours(1).AddSeconds(5);

        // Assert
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var purpose = jwtToken.Claims.Single(claim => claim.Type == TokenService.PurposeClaimType).Value;

        await Assert.That(purpose).IsEqualTo(TokenService.PasswordResetPurpose);
        await Assert.That(jwtToken.ValidTo >= earliestExpiry).IsTrue();
        await Assert.That(jwtToken.ValidTo <= latestExpiry).IsTrue();
    }

    [Test]
    public async Task GivenPasswordResetToken_WhenValidatingPasswordResetToken_ThenReturnsEmail()
    {
        // Arrange
        const string email = "test-user@example.com";
        var tokenService = CreateTokenService();
        var token = tokenService.GeneratePasswordResetToken(email);

        // Act
        var result = tokenService.ValidatePasswordResetToken(token, out var validatedEmail);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(validatedEmail).IsEqualTo(email);
    }

    [Test]
    public async Task GivenEmailConfirmationToken_WhenValidatingPasswordResetToken_ThenFails()
    {
        // Arrange
        var tokenService = CreateTokenService();
        var token = tokenService.GenerateEmailToken("test-user@example.com");

        // Act
        var result = tokenService.ValidatePasswordResetToken(token, out var validatedEmail);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(validatedEmail).IsNull();
    }

    [Test]
    public async Task GivenPasswordResetToken_WhenValidatingEmailToken_ThenFails()
    {
        // Arrange
        var tokenService = CreateTokenService();
        var token = tokenService.GeneratePasswordResetToken("test-user@example.com");

        // Act
        var result = tokenService.ValidateEmailToken(token, out var validatedEmail);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(validatedEmail).IsNull();
    }

    private static TokenService CreateTokenService()
    {
        var options = IOptions<JwtSettings>.Mock();
        options.Value.Returns(new JwtSettings
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SigningKey = "test-signing-key-with-at-least-32-characters",
            LifetimeMinutes = 60
        });

        return new TokenService(options.Object, ILogger<TokenService>.Mock().Object);
    }
}