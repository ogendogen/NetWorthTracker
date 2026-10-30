using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetWorthTracker.Application.Authentication;
using NetWorthTracker.Application.Authentication.Services;
using NetWorthTracker.Application.User.Models.ResetPassword;
using NetWorthTracker.Application.User.UseCases.ResetPassword;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.UnitTests.User.UseCases.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private const string Email = "testmail@test.com";
    private const string NewPassword = "NewPassword1!";

    [Test]
    public async Task GivenValidResetToken_WhenHandlingResetPassword_ThenUpdatesPasswordAndReturnsSuccess()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var tokenService = CreateTokenService();
        var token = tokenService.GeneratePasswordResetToken(Email);
        var userRepoMock = IUserRepository.Mock();
        userRepoMock.UpdatePasswordByEmailAsync(Email, NewPassword, cancellationToken).Returns(true);
        using var services = CreateServices();

        var handler = new ResetPasswordCommandHandler(
            userRepoMock,
            tokenService,
            services,
            ILogger<ResetPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ResetPasswordCommand(token, NewPassword, NewPassword), cancellationToken);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new ResetPasswordResponse(true));
        userRepoMock.UpdatePasswordByEmailAsync(Email, NewPassword, cancellationToken).WasCalled(Times.Once);
    }

    [Test]
    public async Task GivenEmailConfirmationToken_WhenHandlingResetPassword_ThenReturnsInvalidTokenError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var tokenService = CreateTokenService();
        var token = tokenService.GenerateEmailToken(Email);
        var userRepoMock = IUserRepository.Mock();
        using var services = CreateServices();

        var handler = new ResetPasswordCommandHandler(
            userRepoMock,
            tokenService,
            services,
            ILogger<ResetPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ResetPasswordCommand(token, NewPassword, NewPassword), cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.Single().Message).IsEqualTo(ResetPasswordCommandHandler.InvalidTokenError);
        userRepoMock.UpdatePasswordByEmailAsync(Any<string>(), Any<string>(), Any<CancellationToken>()).WasNeverCalled();
    }

    [Test]
    public async Task GivenMalformedToken_WhenHandlingResetPassword_ThenReturnsInvalidTokenError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepoMock = IUserRepository.Mock();
        using var services = CreateServices();

        var handler = new ResetPasswordCommandHandler(
            userRepoMock,
            CreateTokenService(),
            services,
            ILogger<ResetPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("not-a-jwt", NewPassword, NewPassword),
            cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.Single().Message).IsEqualTo(ResetPasswordCommandHandler.InvalidTokenError);
        userRepoMock.UpdatePasswordByEmailAsync(Any<string>(), Any<string>(), Any<CancellationToken>()).WasNeverCalled();
    }

    [Test]
    public async Task GivenValidTokenForUnknownUser_WhenHandlingResetPassword_ThenReturnsInvalidTokenError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var tokenService = CreateTokenService();
        var token = tokenService.GeneratePasswordResetToken(Email);
        var userRepoMock = IUserRepository.Mock();
        userRepoMock.UpdatePasswordByEmailAsync(Email, NewPassword, cancellationToken).Returns(false);
        using var services = CreateServices();

        var handler = new ResetPasswordCommandHandler(
            userRepoMock,
            tokenService,
            services,
            ILogger<ResetPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ResetPasswordCommand(token, NewPassword, NewPassword), cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.Single().Message).IsEqualTo(ResetPasswordCommandHandler.InvalidTokenError);
    }

    [Test]
    public async Task GivenMismatchedPasswords_WhenHandlingResetPassword_ThenReturnsValidationFailure()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var tokenService = CreateTokenService();
        var token = tokenService.GeneratePasswordResetToken(Email);
        var userRepoMock = IUserRepository.Mock();
        using var services = CreateServices();

        var handler = new ResetPasswordCommandHandler(
            userRepoMock,
            tokenService,
            services,
            ILogger<ResetPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand(token, NewPassword, "Different1!"),
            cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        userRepoMock.UpdatePasswordByEmailAsync(Any<string>(), Any<string>(), Any<CancellationToken>()).WasNeverCalled();
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

    private static ServiceProvider CreateServices() =>
        new ServiceCollection()
            .AddSingleton<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>()
            .BuildServiceProvider();
}
