using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.IntegrationTests;

public class AuthenticationTests
{
    [ClassDataSource<NetWorthTrackerApiFactory>(Shared = SharedType.PerTestSession)]
    public required NetWorthTrackerApiFactory ApiFactory { get; init; }

    [Test]
    public async Task GivenValidCredentials_WhenLoggingIn_ThenReturnsValidSignedToken()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();
        var earliestExpiry = DateTimeOffset.UtcNow.AddMinutes(15);

        // Act
        var response = await client.PostAsJsonAsync(
            "/login",
            new LoginRequest(NetWorthTrackerApiFactory.TestUserName, NetWorthTrackerApiFactory.TestPassword));
        var latestExpiry = DateTimeOffset.UtcNow.AddMinutes(15);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>()
                            ?? throw new InvalidOperationException("Login response content is required.");
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(NetWorthTrackerApiFactory.JwtSigningKey)),
            ValidIssuer = NetWorthTrackerApiFactory.JwtIssuer,
            ValidAudience = NetWorthTrackerApiFactory.JwtAudience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name
        };
        var principal = tokenHandler.ValidateToken(
            loginResponse.AccessToken,
            validationParameters,
            out var validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;

        await Assert.That(loginResponse.UserName).IsEqualTo(NetWorthTrackerApiFactory.TestUserName);
        await Assert.That(string.IsNullOrWhiteSpace(loginResponse.AccessToken)).IsFalse();
        await Assert.That(loginResponse.ExpiresAt >= earliestExpiry).IsTrue();
        await Assert.That(loginResponse.ExpiresAt <= latestExpiry).IsTrue();
        await Assert.That(
            Math.Abs((loginResponse.ExpiresAt.UtcDateTime - jwtToken.ValidTo).TotalSeconds) <= 1).IsTrue();
        await Assert.That(jwtToken.Header.Alg).IsEqualTo(SecurityAlgorithms.HmacSha256);
        await Assert.That(jwtToken.Issuer).IsEqualTo(NetWorthTrackerApiFactory.JwtIssuer);
        await Assert.That(jwtToken.Audiences).Contains(NetWorthTrackerApiFactory.JwtAudience);
        await Assert.That(jwtToken.Subject).IsEqualTo(NetWorthTrackerApiFactory.TestUserName);
        await Assert.That(principal.Identity?.Name).IsEqualTo(NetWorthTrackerApiFactory.TestUserName);
    }

    [Test]
    public async Task GivenIncorrectPassword_WhenLoggingIn_ThenReturnsUnauthorized()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/login",
            new LoginRequest(NetWorthTrackerApiFactory.TestUserName, "incorrect-password"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GivenUnknownUser_WhenLoggingIn_ThenReturnsUnauthorized()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/login",
            new LoginRequest("unknown-user", NetWorthTrackerApiFactory.TestPassword));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}