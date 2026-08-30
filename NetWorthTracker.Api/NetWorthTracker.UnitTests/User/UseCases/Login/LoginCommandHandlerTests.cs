using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.UseCases.Login;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.UnitTests.User.UseCases.Login;

public class LoginCommandHandlerTests
{
    [Test]
    public async Task GivenValidCredentials_WhenHandlingLogin_ThenReturnsTokenResponse()
    {
        // Arrange
        const string username = "test-user";
        const string password = "valid-password";
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = new LoginResponse("access-token", DateTimeOffset.UtcNow.AddHours(1), username);
        var userRepository = IUserRepository.Mock();
        var tokenService = ITokenService.Mock();
        userRepository.LoginAsync(username, password, cancellationToken).Returns(true);
        tokenService.CreateLoginResponse(username).Returns(expectedResponse);
        var handler = new LoginCommandHandler(userRepository.Object, tokenService.Object);
        var command = new LoginCommand(username, password);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result).IsEqualTo(expectedResponse);
        userRepository.LoginAsync(username, password, cancellationToken).WasCalled(Times.Once);
        tokenService.CreateLoginResponse(username).WasCalled(Times.Once);
    }

    [Test]
    public async Task GivenInvalidCredentials_WhenHandlingLogin_ThenReturnsNull()
    {
        // Arrange
        const string username = "test-user";
        const string password = "invalid-password";
        var cancellationToken = new CancellationTokenSource().Token;
        var userRepository = IUserRepository.Mock();
        var tokenService = ITokenService.Mock();
        userRepository.LoginAsync(username, password, cancellationToken).Returns(false);
        var handler = new LoginCommandHandler(userRepository.Object, tokenService.Object);
        var command = new LoginCommand(username, password);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result).IsNull();
        userRepository.LoginAsync(username, password, cancellationToken).WasCalled(Times.Once);
        tokenService.CreateLoginResponse(Any<string>()).WasNeverCalled();
    }
}