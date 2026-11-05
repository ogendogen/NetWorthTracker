using FluentValidation;

namespace NetWorthTracker.Application.User.UseCases.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Provided email cannot be empty")
            .EmailAddress().WithMessage("Provided email is not a valid email address")
            .MaximumLength(320).WithMessage("Provided email address cannot be longer than 320 characters");
    }
}
