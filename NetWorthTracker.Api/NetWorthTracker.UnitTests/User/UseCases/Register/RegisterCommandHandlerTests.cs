using FluentResults;
using NetWorthTracker.Application.User.Models.Register;
using NetWorthTracker.Application.User.UseCases.Register;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.UnitTests.User.UseCases.Register;

public class RegisterCommandHandlerTests
{
    [Test]
    public async Task GivenValidCredentials_WhenHandlingRegistration_ThenReturnsResultValue()
    {
        // Arrange
        const string username = "username";
        const string password = "password";
        const string email = "testmail@test.com";
        var expectedResponse = new RegisterResponse(Success: true);
        var cancellationToken = new CancellationTokenSource().Token;

        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByUsernameOrEmailAsync(username, email, cancellationToken)
            .Returns(null as Domain.User.Models.User);
        userRepoMock.RegisterAsync(username, password, email,
            cancellationToken).Returns(true);

        var registerCommandHandler = new RegisterCommandHandler(userRepoMock);
        var command = new RegisterCommand(username, password, email);

        // Act
        var result = await registerCommandHandler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsFalse();

        var value = result.Value;
        await Assert.That(value).IsNotNull();
        await Assert.That(value).IsEqualTo(expectedResponse);

        userRepoMock.GetByUsernameOrEmailAsync(username, email,
            cancellationToken).WasCalled(Times.Once);
        userRepoMock.RegisterAsync(username, password, email,
            cancellationToken).WasCalled(Times.Once);
    }

    [Test]
    public async Task GivenExistingUserCredentials_WhenHandlingRegistration_ThenReturnsErrorResult()
    {
        // Arrange
        const string username = "username";
        const string password = "password";
        const string email = "testmail@test.com";
        const string expectedErrorMessage = "User with provided credentials already exists.";
        var cancellationToken = new CancellationTokenSource().Token;

        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByUsernameOrEmailAsync(username, email, cancellationToken)
            .Returns(new Domain.User.Models.User(Guid.NewGuid(), username, password, email, true, DateTimeOffset.Now));

        var registerCommandHandler = new RegisterCommandHandler(userRepoMock);
        var command = new RegisterCommand(username, password, email);

        // Act
        var result = await registerCommandHandler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();

        var error = result.Errors.Single();
        await Assert.That(error).IsNotNull();
        await Assert.That(error.Message).IsEqualTo(expectedErrorMessage);

        userRepoMock.GetByUsernameOrEmailAsync(username, email,
            cancellationToken).WasCalled(Times.Once);
        userRepoMock.RegisterAsync(username, password, email,
            cancellationToken).WasNeverCalled();
    }

    [Test]
    public async Task GivenValidCredentials_WhenRegistrationFails_ThenReturnsError()
    {
        // Arrange
        const string username = "username";
        const string password = "password";
        const string email = "testmail@test.com";
        const string expectedErrorMessage = "User registration failed.";

        var cancellationToken = new CancellationTokenSource().Token;

        var userRepoMock = IUserRepository.Mock();
        userRepoMock.GetByUsernameOrEmailAsync(username, email, cancellationToken)
            .Returns(null as Domain.User.Models.User);
        userRepoMock.RegisterAsync(username, password, email,
            cancellationToken).Returns(false);

        var registerCommandHandler = new RegisterCommandHandler(userRepoMock);
        var command = new RegisterCommand(username, password, email);

        // Act
        var result = await registerCommandHandler.Handle(command, cancellationToken);

        // Assert
        await Assert.That(result.IsFailed).IsTrue();

        var error = result.Errors.Single();
        await Assert.That(error).IsNotNull();
        await Assert.That(error.Message).IsEqualTo(expectedErrorMessage);

        userRepoMock.GetByUsernameOrEmailAsync(username, email,
            cancellationToken).WasCalled(Times.Once);
        userRepoMock.RegisterAsync(username, password, email,
            cancellationToken).WasCalled(Times.Once);
    }
}