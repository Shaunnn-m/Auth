using FluentValidation;

namespace Authentication.Application.Features.Authentication
    .ForgotPassword;

public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage(
                "A valid email address is required.");
    }
}