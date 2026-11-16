using FluentValidation;

using NetWorthTracker.Application.Common.Validation;

namespace NetWorthTracker.Application.User.UseCases.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        ConfigureUsernameRules();
        ConfigurePasswordRules();
    }

    private void ConfigureUsernameRules()
    {
        RuleFor(x => x.Username)
            .ApplyUsernameRules();
    }

    private void ConfigurePasswordRules()
    {
        RuleFor(x => x.Password)
            .ApplyUserLoginPasswordRules();
    }
}
