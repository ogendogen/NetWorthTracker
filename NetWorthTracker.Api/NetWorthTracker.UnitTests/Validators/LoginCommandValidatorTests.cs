using FluentValidation;
using NetWorthTracker.Application.User.UseCases.Login;

namespace NetWorthTracker.UnitTests.User.UseCases.Login;

public class LoginCommandValidatorTests : ValidatorTestsBase
{
    [Test]
    public async Task GivenValidLoginCommand_WhenValidating_ThenDoesNotThrow()
    {
        var command = new LoginCommand("username", "Password1!");

        await Assert.That(async () => await new LoginCommandValidator().ValidateAndThrowAsync(command))
            .ThrowsNothing();
    }

    [Test]
    public async Task GivenLoginCommandWithEmptyUsername_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new LoginCommand(string.Empty, "Password1!");

        await AssertValidationErrorsHaveMessagesAsync(new LoginCommandValidator(), command,
            "Provided username cannot be empty");
    }

    [Test]
    public async Task GivenLoginCommandWithTooLongUsername_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new LoginCommand(new string('u', 33), "Password1!");

        await AssertValidationErrorsHaveMessagesAsync(new LoginCommandValidator(), command,
            "Provided login cannot be longer than 32 characters");
    }

    [Test]
    public async Task GivenLoginCommandWithEmptyPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new LoginCommand("username", string.Empty);

        await AssertValidationErrorsHaveMessagesAsync(new LoginCommandValidator(), command,
            "Provided password cannot be empty");
    }

    [Test]
    public async Task GivenLoginCommandWithTooLongPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new LoginCommand("username", new string('p', 65));

        await AssertValidationErrorsHaveMessagesAsync(new LoginCommandValidator(), command,
            "Provided password cannot be longer than 64 characters");
    }
}
