using FluentValidation;

namespace Authentication.Application.Features.Authentication.ActivateUser;

public sealed class ActivateUserValidator
    : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}