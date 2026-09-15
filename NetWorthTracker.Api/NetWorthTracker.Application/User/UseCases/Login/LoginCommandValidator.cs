using FluentValidation;

namespace NetWorthTracker.Application.User.UseCases.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Provided username cannot be empty")
            .MaximumLength(32).WithMessage("Provided login cannot be longer than 32 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Provided password cannot be empty")
            .MaximumLength(64).WithMessage("Provided password cannot be longer than 64 characters");
    }
}
