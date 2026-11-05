using FluentValidation;

namespace NetWorthTracker.Application.Common.Validation;

public static class PasswordRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Provided password cannot be empty")
            .MinimumLength(8).WithMessage("Provided password must have at least 8 characters")
            .MaximumLength(64).WithMessage("Provided password cannot be longer than 64 characters")
            .Must(ContainsUppercase).WithMessage("Provided password must contain at least one uppercase letter")
            .Must(ContainsNumber).WithMessage("Provided password must contain at least one number")
            .Must(ContainsSpecialCharacter)
            .WithMessage("Provided password must contain at least one special character");

    private static bool ContainsUppercase(string? password) =>
        password?.Any(char.IsUpper) == true;

    private static bool ContainsNumber(string? password) =>
        password?.Any(char.IsDigit) == true;

    private static bool ContainsSpecialCharacter(string? password) =>
        password?.Any(character => char.IsPunctuation(character) || char.IsSymbol(character)) == true;
}
