using FluentValidation;

using NetWorthTracker.Application.Common.Validation;

namespace NetWorthTracker.Application.User.UseCases.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .ApplyEmailRules();
    }
}
