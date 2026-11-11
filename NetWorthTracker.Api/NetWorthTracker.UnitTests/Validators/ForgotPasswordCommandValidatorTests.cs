using FluentValidation;
using NetWorthTracker.Application.User.UseCases.ForgotPassword;
using NetWorthTracker.UnitTests.User.UseCases;

namespace NetWorthTracker.UnitTests.Validators;

public class ForgotPasswordCommandValidatorTests : ValidatorTestsBase
{
    [Test]
    public async Task GivenValidEmail_WhenValidating_ThenDoesNotThrow()
    {
        var command = new ForgotPasswordCommand("testmail@test.com");

        await Assert.That(async () => await new ForgotPasswordCommandValidator().ValidateAndThrowAsync(command))
            .ThrowsNothing();
    }

    [Test]
    public async Task GivenEmptyEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ForgotPasswordCommand(string.Empty);

        await AssertValidationErrorsHaveMessagesAsync(new ForgotPasswordCommandValidator(), command,
            "Provided email cannot be empty");
    }

    [Test]
    public async Task GivenInvalidEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var command = new ForgotPasswordCommand("not-an-email");

        await AssertValidationErrorsHaveMessagesAsync(new ForgotPasswordCommandValidator(), command,
            "Provided email is not a valid email address");
    }

    [Test]
    public async Task GivenTooLongEmail_WhenValidating_ThenValidationErrorHasExpectedMessage()
    {
        var email = new string('a', EmailMaxLength - "@test.com".Length + 1) + "@test.com";
        var command = new ForgotPasswordCommand(email);

        await AssertValidationErrorsHaveMessagesAsync(new ForgotPasswordCommandValidator(), command,
            "Provided email address cannot be longer than 320 characters");
    }

    [Test]
    public async Task GivenEmailAtMaximumLength_WhenValidating_ThenDoesNotThrow()
    {
        var email = new string('a', EmailMaxLength - "@test.com".Length) + "@test.com";
        var command = new ForgotPasswordCommand(email);

        await Assert.That(async () => await new ForgotPasswordCommandValidator().ValidateAndThrowAsync(command))
            .ThrowsNothing();
    }
}
