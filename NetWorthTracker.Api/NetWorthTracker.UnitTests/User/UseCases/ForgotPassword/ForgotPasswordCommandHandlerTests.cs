using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.User.Models.ForgotPassword;
using NetWorthTracker.Application.User.UseCases.ForgotPassword;
using NetWorthTracker.Domain.Common.Interfaces;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.UnitTests.User.UseCases.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private const string Username = "username";
    private const string Email = "testmail@test.com";
    private const string ResetToken = "reset-token";

    [Test]
    public async Task GivenConfirmedUser_WhenHandlingForgotPassword_ThenSendsResetEmailAndReturnsSuccess()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByEmailAsync(Email, cancellationToken).Returns(CreateUser(isEmailConfirmed: true));
        var tokenService = ITokenService.Mock();
        tokenService.GeneratePasswordResetToken(Email).Returns(ResetToken);
        var emailService = IEmailService.Mock();
        using var services = CreateServices();

        var handler = new ForgotPasswordCommandHandler(
            userRepoMock,
            emailService.Object,
            tokenService.Object,
            services,
            ILogger<ForgotPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ForgotPasswordCommand(Email), cancellationToken);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new ForgotPasswordResponse(true));
        tokenService.GeneratePasswordResetToken(Email).WasCalled(Times.Once);
        emailService.SendPasswordResetEmail(Username, Email, ResetToken).WasCalled(Times.Once);
    }

    [Test]
    public async Task GivenUnconfirmedUser_WhenHandlingForgotPassword_ThenDoesNotSendEmailButReturnsSuccess()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByEmailAsync(Email, cancellationToken).Returns(CreateUser(isEmailConfirmed: false));
        var tokenService = ITokenService.Mock();
        var emailService = IEmailService.Mock();
        using var services = CreateServices();

        var handler = new ForgotPasswordCommandHandler(
            userRepoMock,
            emailService.Object,
            tokenService.Object,
            services,
            ILogger<ForgotPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ForgotPasswordCommand(Email), cancellationToken);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new ForgotPasswordResponse(true));
        tokenService.GeneratePasswordResetToken(Any<string>()).WasNeverCalled();
        emailService.SendPasswordResetEmail(Any<string>(), Any<string>(), Any<string>()).WasNeverCalled();
    }

    [Test]
    public async Task GivenUnknownEmail_WhenHandlingForgotPassword_ThenDoesNotSendEmailButReturnsSuccess()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByEmailAsync(Email, cancellationToken).Returns(null as Domain.User.Models.User);
        var tokenService = ITokenService.Mock();
        var emailService = IEmailService.Mock();
        using var services = CreateServices();

        var handler = new ForgotPasswordCommandHandler(
            userRepoMock,
            emailService.Object,
            tokenService.Object,
            services,
            ILogger<ForgotPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ForgotPasswordCommand(Email), cancellationToken);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new ForgotPasswordResponse(true));
        tokenService.GeneratePasswordResetToken(Any<string>()).WasNeverCalled();
        emailService.SendPasswordResetEmail(Any<string>(), Any<string>(), Any<string>()).WasNeverCalled();
    }

    [Test]
    public async Task GivenInvalidEmail_WhenHandlingForgotPassword_ThenReturnsValidationFailure()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepoMock = IUserRepository.Mock();
        var tokenService = ITokenService.Mock();
        var emailService = IEmailService.Mock();
        using var services = CreateServices();

        var handler = new ForgotPasswordCommandHandler(
            userRepoMock,
            emailService.Object,
            tokenService.Object,
            services,
            ILogger<ForgotPasswordCommandHandler>.Mock().Object);

        // Act
        var result = await handler.Handle(new ForgotPasswordCommand("not-an-email"), cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();
        userRepoMock.GetByEmailAsync(Any<string>(), Any<CancellationToken>()).WasNeverCalled();
    }

    private static Domain.User.Models.User CreateUser(bool isEmailConfirmed) => new()
    {
        UserId = Guid.NewGuid(),
        Login = Username,
        PasswordHash = "hash",
        Email = Email,
        IsEmailConfirmed = isEmailConfirmed,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static ServiceProvider CreateServices() =>
        new ServiceCollection()
            .AddSingleton<IValidator<ForgotPasswordCommand>, ForgotPasswordCommandValidator>()
            .BuildServiceProvider();
}
