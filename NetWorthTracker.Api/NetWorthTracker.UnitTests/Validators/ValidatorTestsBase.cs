using FluentValidation;

using NetWorthTracker.Application.Common.Validation;

namespace NetWorthTracker.UnitTests.User.UseCases;

public class ValidatorTestsBase
{
    protected static readonly int UsernameMaxLength = AccountValidationLimits.UsernameMaxLength;
    protected static readonly int EmailMaxLength = AccountValidationLimits.EmailMaxLength;
    protected static readonly int PasswordMinLength = AccountValidationLimits.PasswordMinLength;
    protected static readonly int PasswordMaxLength = AccountValidationLimits.PasswordMaxLength;

    protected static async Task AssertValidationErrorsHaveMessagesAsync<T>(IValidator<T> validator, T command,
        string expectedMessage)
    {
        var validationException = await Assert.That(async () => await validator.ValidateAndThrowAsync(command))
            .Throws<ValidationException>();

        await Assert.That(validationException!.Errors).IsNotEmpty();

        var error = validationException.Errors.First();

        await Assert.That(error.ErrorMessage.Contains(expectedMessage)).IsTrue();
    }
}
