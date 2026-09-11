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
            .NotEmpty().WithMessage("Provided password cannot be empty");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Provided email cannot be empty")
            .EmailAddress().WithMessage("Provided email is not a valid email address")
            .MaximumLength(320).WithMessage("Provided email address cannot be longer than 320 characters");
    }
}