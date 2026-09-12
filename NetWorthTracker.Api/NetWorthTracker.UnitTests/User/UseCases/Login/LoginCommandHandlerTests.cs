using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.UseCases.Login;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.UnitTests.User.UseCases.Login;

public class LoginCommandHandlerTests
{
    [Test]
    public async Task GivenInvalidCredentials_WhenHandlingLogin_ThenReturnsErrorResult()
    {
        // Arrange
        const string username = "test-user";
        const string password = "invalid-password";
        const string expectedErrorMessage = "User with provided credentials does not exist.";
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepository = IUserRepository.Mock();
        var tokenService = ITokenService.Mock();
        var logger = ILogger<LoginCommandHandler>.Mock();
        userRepository.LoginAsync(username, password, cancellationToken).Returns(false);
        var handler = new LoginCommandHandler(
            userRepository.Object,
            tokenService.Object,
            logger: logger.Object);
        var command = new LoginCommand(username, password);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();

        var error = result.Errors.Single();
        await Assert.That(error).IsNotNull();
        await Assert.That(error.Message).IsEqualTo(expectedErrorMessage);
        userRepository.LoginAsync(username, password, cancellationToken).WasCalled(Times.Once);
        tokenService.CreateLoginResponse(Any<string>()).WasNeverCalled();
    }
}