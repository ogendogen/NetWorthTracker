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
            .ApplyUsernameRules();
    }

    private void ConfigurePasswordRules()
    {
        RuleFor(x => x.Password).ApplyUserRegisterPasswordRules();
    }

    private void ConfigureEmailRules()
    {
        RuleFor(x => x.Email)
            .ApplyEmailRules();
    }
}
