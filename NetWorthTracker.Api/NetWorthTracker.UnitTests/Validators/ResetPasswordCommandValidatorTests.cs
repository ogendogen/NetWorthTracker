using FluentValidation;
using NetWorthTracker.Application.User.UseCases.ResetPassword;
using NetWorthTracker.UnitTests.User.UseCases;

namespace NetWorthTracker.UnitTests.Validators;

public class ResetPasswordCommandValidatorTests : ValidatorTestsBase
{
    private const string Token = "token";
    private const string Password = "Password1!";

    [Test]
    public async Task GivenValidResetPasswordCommand_WhenValidating_ThenDoesNotThrow()
    {
        var command = new ResetPasswordCommand(Token, Password, Password);

        await Assert.That(async () => await new ResetPasswordCommandValidator().ValidateAndThrowAsync(command))
            .ThrowsNothing();
    }

    [Test]
    public async Task GivenEmptyToken_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(string.Empty, Password, Password);

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided token cannot be empty");
    }

    [Test]
    public async Task GivenEmptyPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(Token, string.Empty, string.Empty);

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password cannot be empty");
    }

    [Test]
    public async Task GivenPasswordWithoutUppercase_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(Token, "password1!", "password1!");

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password must contain at least one uppercase letter");
    }

    [Test]
    public async Task GivenPasswordWithoutNumber_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(Token, "Password!!", "Password!!");

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password must contain at least one number");
    }

    [Test]
    public async Task GivenPasswordWithoutSpecialCharacter_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(Token, "Password11", "Password11");

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password must contain at least one special character");
    }

    [Test]
    public async Task GivenMismatchedPasswords_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ResetPasswordCommand(Token, Password, "Different1!");

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Passwords do not match");
    }

    [Test]
    public async Task GivenTooShortPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var password = "A1!" + new string('a', PasswordMinLength - "A1!".Length - 1);
        var command = new ResetPasswordCommand(Token, password, password);

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password must have at least 8 characters");
    }

    [Test]
    public async Task GivenTooLongPassword_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var password = "A1!" + new string('a', PasswordMaxLength - "A1!".Length + 1);
        var command = new ResetPasswordCommand(Token, password, password);

        await AssertValidationErrorsHaveMessagesAsync(new ResetPasswordCommandValidator(), command,
            "Provided password cannot be longer than 64 characters");
    }
}
