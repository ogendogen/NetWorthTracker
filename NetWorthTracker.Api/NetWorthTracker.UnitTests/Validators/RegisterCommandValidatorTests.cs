using FluentValidation;
using NetWorthTracker.Application.User.UseCases.Register;

namespace NetWorthTracker.UnitTests.User.UseCases.Register;

public class RegisterCommandValidatorTests : ValidatorTestsBase
{
    [Test]
    public async Task GivenValidRegisterCommand_WhenValidating_ThenDoesNotThrow()
    {
        var command = new RegisterCommand("username", "Password1!", "testmail@test.com");

        await Assert.That(async () => await new RegisterCommandValidator().ValidateAndThrowAsync(command))
            .ThrowsNothing();
    }

    [Test]
    public async Task GivenRegisterCommandWithEmptyUsername_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand(string.Empty, "Password1!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided username cannot be empty");
    }

    [Test]
    public async Task GivenRegisterCommandWithTooLongUsername_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand(new string('u', 33), "Password1!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided login cannot be longer than 32 characters");
    }

    [Test]
    public async Task GivenRegisterCommandWithEmptyPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", string.Empty, "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password cannot be empty");
    }

    [Test]
    public async Task GivenRegisterCommandWithTooShortPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Aa1!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password must have at least 8 characters");
    }

    [Test]
    public async Task GivenRegisterCommandWithTooLongPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", new string('a', 62) + "A1!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password cannot be longer than 64 characters");
    }

    [Test]
    public async Task GivenRegisterCommandWithoutUppercaseLetter_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "password1!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password must contain at least one uppercase letter");
    }

    [Test]
    public async Task GivenRegisterCommandWithoutNumber_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Password!", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password must contain at least one number");
    }

    [Test]
    public async Task GivenRegisterCommandWithoutSpecialCharacter_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Password1", "testmail@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided password must contain at least one special character");
    }

    [Test]
    public async Task GivenRegisterCommandWithEmptyEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Password1!", string.Empty);

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided email cannot be empty");
    }

    [Test]
    public async Task GivenRegisterCommandWithInvalidEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Password1!", "invalid-email");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided email is not a valid email address");
    }

    [Test]
    public async Task GivenRegisterCommandWithTooLongEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new RegisterCommand("username", "Password1!", new string('a', 312) + "@test.com");

        await AssertValidationErrorsHaveMessagesAsync(new RegisterCommandValidator(), command,
            "Provided email address cannot be longer than 320 characters");
    }
}
