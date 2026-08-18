using FluentValidation;

namespace Authentication.Application.Features.Authentication.Logout;

public sealed class LogoutValidator
    : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}