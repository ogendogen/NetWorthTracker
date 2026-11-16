using FluentValidation;

namespace NetWorthTracker.Application.Common.Validation;

public static class UserRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyUsernameRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Provided username cannot be empty")
            .MaximumLength(AccountValidationLimits.UsernameMaxLength)
            .WithMessage($"Provided username cannot be longer than {AccountValidationLimits.UsernameMaxLength} characters");

    public static IRuleBuilderOptions<T, string> ApplyEmailRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Provided email cannot be empty")
            .EmailAddress().WithMessage("Provided email is not a valid email address")
            .MaximumLength(AccountValidationLimits.EmailMaxLength)
            .WithMessage($"Provided email address cannot be longer than {AccountValidationLimits.EmailMaxLength} characters");
}
