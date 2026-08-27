using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NetWorthTracker.Api.Models;
using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.IntegrationTests;

public class DataEndpointTests
{
    [ClassDataSource<NetWorthTrackerApiFactory>(Shared = SharedType.PerTestSession)]
    public required NetWorthTrackerApiFactory ApiFactory { get; init; }

    [Test]
    public async Task GivenAnonymousRequest_WhenGettingData_ThenReturnsUnauthorized()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/data");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GivenAuthenticatedRequest_WhenGettingData_ThenReturnsNetWorthSummary()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();
        var loginResponse = await LoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
        var earliestUpdatedAt = DateTimeOffset.UtcNow;

        // Act
        var response = await client.GetAsync("/data");
        var latestUpdatedAt = DateTimeOffset.UtcNow;

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<NetWorthSummary>()
                      ?? throw new InvalidOperationException("Net worth summary content is required.");
        await Assert.That(summary.NetWorth).IsEqualTo(125_500m);
        await Assert.That(summary.Assets).IsEqualTo(168_750m);
        await Assert.That(summary.Liabilities).IsEqualTo(43_250m);
        await Assert.That(summary.UpdatedAt >= earliestUpdatedAt).IsTrue();
        await Assert.That(summary.UpdatedAt <= latestUpdatedAt).IsTrue();
    }

    [Test]
    public async Task GivenTamperedToken_WhenGettingData_ThenReturnsUnauthorized()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();
        var loginResponse = await LoginAsync(client);
        var tokenParts = loginResponse.AccessToken.Split('.');
        var signature = tokenParts[2];
        var replacement = signature[0] == 'A' ? 'B' : 'A';
        tokenParts[2] = $"{replacement}{signature[1..]}";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", string.Join('.', tokenParts));

        // Act
        var response = await client.GetAsync("/data");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    private static async Task<LoginResponse> LoginAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/login",
            new LoginRequest(NetWorthTrackerApiFactory.TestUserName, NetWorthTrackerApiFactory.TestPassword));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>()
               ?? throw new InvalidOperationException("Login response content is required.");
    }
}