using System.Net;
using System.Net.Http.Json;
using NetWorthTracker.Application.User.Models.Register;

namespace NetWorthTracker.IntegrationTests;

public class RegisterEndpointTests
{
    [ClassDataSource<NetWorthTrackerApiFactory>(Shared = SharedType.PerTestSession)]
    public required NetWorthTrackerApiFactory ApiFactory { get; init; }

    [Test]
    public async Task GivenValidRequest_WhenRegisteringNewUser_ThenReturnsOk()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/register",
            new RegisterRequest("user2137", "Password7312!",
                "newmail@email.com"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        await Assert.That(result?.Success).IsTrue();
    }
}