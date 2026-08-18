using FluentValidation;

namespace Authentication.Application.Features.Authentication.Refresh;

public sealed class RefreshTokenValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}