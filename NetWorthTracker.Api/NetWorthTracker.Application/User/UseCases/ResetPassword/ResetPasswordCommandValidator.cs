using FluentValidation;
using NetWorthTracker.Application.Common.Validation;

namespace NetWorthTracker.Application.User.UseCases.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Provided token cannot be empty");

        RuleFor(x => x.Password).ApplyPasswordRules();

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
