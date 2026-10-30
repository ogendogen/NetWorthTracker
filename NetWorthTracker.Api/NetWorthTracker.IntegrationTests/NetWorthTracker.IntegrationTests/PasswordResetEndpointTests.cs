using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetWorthTracker.Application.User.Models.ForgotPassword;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Application.User.Models.ResetPassword;
using NetWorthTracker.Infrastructure;

namespace NetWorthTracker.IntegrationTests;

public class PasswordResetEndpointTests
{
    [ClassDataSource<NetWorthTrackerApiFactory>(Shared = SharedType.PerTestSession)]
    public required NetWorthTrackerApiFactory ApiFactory { get; init; }

    [Test]
    public async Task GivenConfirmedUserEmail_WhenRequestingPasswordReset_ThenReturnsOkAndSendsResetEmail()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/forgot-password",
            new ForgotPasswordRequest(NetWorthTrackerApiFactory.TestUserEmail));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
        await Assert.That(result?.Success).IsTrue();

        var resetEmail = ApiFactory.EmailService.SentEmails.LastOrDefault(email =>
            email.Kind == RecordingEmailService.PasswordResetKind &&
            email.ToAddress == NetWorthTrackerApiFactory.TestUserEmail);
        await Assert.That(resetEmail).IsNotNull();
        await Assert.That(resetEmail!.Username).IsEqualTo(NetWorthTrackerApiFactory.TestUserName);
        await Assert.That(string.IsNullOrWhiteSpace(resetEmail.Token)).IsFalse();
    }

    [Test]
    public async Task GivenUnknownEmail_WhenRequestingPasswordReset_ThenReturnsSameOkResponseWithoutEmail()
    {
        // Arrange
        const string unknownEmail = "nobody-here@example.com";
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/forgot-password", new ForgotPasswordRequest(unknownEmail));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
        await Assert.That(result?.Success).IsTrue();
        await Assert.That(ApiFactory.EmailService.SentEmails.Any(email => email.ToAddress == unknownEmail)).IsFalse();
    }

    [Test]
    public async Task GivenInvalidEmail_WhenRequestingPasswordReset_ThenReturnsBadRequest()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/forgot-password", new ForgotPasswordRequest("not-an-email"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GivenResetTokenFromEmail_WhenResettingPassword_ThenNewPasswordLogsInAndOldOneDoesNot()
    {
        // Arrange
        const string username = "reset-flow-user";
        const string email = "reset-flow-user@example.com";
        const string oldPassword = "OldPassword1!";
        const string newPassword = "NewPassword2@";
        using var client = ApiFactory.CreateClient();
        await RegisterConfirmedUserAsync(client, username, email, oldPassword);

        var forgotResponse = await client.PostAsJsonAsync("/forgot-password", new ForgotPasswordRequest(email));
        await Assert.That(forgotResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var resetEmail = ApiFactory.EmailService.SentEmails.Last(sent =>
            sent.Kind == RecordingEmailService.PasswordResetKind && sent.ToAddress == email);
        var token = Uri.UnescapeDataString(resetEmail.Token);

        // Act
        var resetResponse = await client.PostAsJsonAsync(
            "/reset-password",
            new ResetPasswordRequest(token, newPassword, newPassword));

        // Assert
        await Assert.That(resetResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var resetResult = await resetResponse.Content.ReadFromJsonAsync<ResetPasswordResponse>();
        await Assert.That(resetResult?.Success).IsTrue();

        var newLogin = await client.PostAsJsonAsync("/login", new LoginRequest(username, newPassword));
        await Assert.That(newLogin.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var oldLogin = await client.PostAsJsonAsync("/login", new LoginRequest(username, oldPassword));
        await Assert.That(oldLogin.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GivenMismatchedPasswords_WhenResettingPassword_ThenReturnsBadRequest()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();
        await client.PostAsJsonAsync(
            "/forgot-password",
            new ForgotPasswordRequest(NetWorthTrackerApiFactory.TestUserEmail));
        var resetEmail = ApiFactory.EmailService.SentEmails.Last(sent =>
            sent.Kind == RecordingEmailService.PasswordResetKind &&
            sent.ToAddress == NetWorthTrackerApiFactory.TestUserEmail);
        var token = Uri.UnescapeDataString(resetEmail.Token);

        // Act
        var response = await client.PostAsJsonAsync(
            "/reset-password",
            new ResetPasswordRequest(token, "NewPassword2@", "Different3#"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        var stillValidLogin = await client.PostAsJsonAsync(
            "/login",
            new LoginRequest(NetWorthTrackerApiFactory.TestUserName, NetWorthTrackerApiFactory.TestPassword));
        await Assert.That(stillValidLogin.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task GivenMalformedToken_WhenResettingPassword_ThenReturnsBadRequest()
    {
        // Arrange
        using var client = ApiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/reset-password",
            new ResetPasswordRequest("not-a-valid-token", "NewPassword2@", "NewPassword2@"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    private async Task RegisterConfirmedUserAsync(HttpClient client, string username, string email, string password)
    {
        var registerResponse = await client.PostAsJsonAsync(
            "/register",
            new RegisterRequest(username, password, email));
        await Assert.That(registerResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);

        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetWorthTrackerDbContext>();
        await dbContext.Users
            .Where(user => user.Email == email)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.IsEmailConfirmed, true));
    }
}
