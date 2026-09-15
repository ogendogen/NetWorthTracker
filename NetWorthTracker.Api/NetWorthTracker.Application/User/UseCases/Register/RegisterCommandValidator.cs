using FluentValidation;

namespace NetWorthTracker.Application.User.UseCases.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Provided username cannot be empty")
            .MaximumLength(32).WithMessage("Provided login cannot be longer than 32 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Provided password cannot be empty")
            .MinimumLength(8).WithMessage("Provided password must have at least 8 characters")
            .MaximumLength(64).WithMessage("Provided password cannot be longer than 64 characters")
            .Must(ContainsUppercase).WithMessage("Provided password must contain at least one uppercase letter")
            .Must(ContainsNumber).WithMessage("Provided password must contain at least one number")
            .Must(ContainsSpecialCharacter).WithMessage("Provided password must contain at least one special character");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Provided email cannot be empty")
            .EmailAddress().WithMessage("Provided email is not a valid email address")
            .MaximumLength(320).WithMessage("Provided email address cannot be longer than 320 characters");
    }

    private static bool ContainsUppercase(string? password) =>
        password?.Any(char.IsUpper) == true;

    private static bool ContainsNumber(string? password) =>
        password?.Any(char.IsDigit) == true;

    private static bool ContainsSpecialCharacter(string? password) =>
        password?.Any(character => char.IsPunctuation(character) || char.IsSymbol(character)) == true;
}
