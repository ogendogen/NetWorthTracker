using FluentValidation;
using NetWorthTracker.Application.Common.Validation;

namespace NetWorthTracker.Application.User.UseCases.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        ConfigureUsernameRules();
        ConfigurePasswordRules();
        ConfigureEmailRules();
    }

    private void ConfigureUsernameRules()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Provided username cannot be empty")
            .MaximumLength(32).WithMessage("Provided username cannot be longer than 32 characters");
    }

    private void ConfigurePasswordRules()
    {
        RuleFor(x => x.Password).ApplyPasswordRules();
    }

    private void ConfigureEmailRules()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Provided email cannot be empty")
            .EmailAddress().WithMessage("Provided email is not a valid email address")
            .MaximumLength(320).WithMessage("Provided email address cannot be longer than 320 characters");
    }
}