using FluentValidation;

namespace NetWorthTracker.Application.User.UseCases.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Provided username cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Provided password cannot be empty");
    }
}
