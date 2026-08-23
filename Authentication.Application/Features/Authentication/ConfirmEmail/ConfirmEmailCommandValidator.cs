using FluentValidation;

namespace Authentication.Application.Features.Authentication.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator
    : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(command => command.Token)
            .NotEmpty()
            .WithMessage("Confirmation token is required.");
    }
}