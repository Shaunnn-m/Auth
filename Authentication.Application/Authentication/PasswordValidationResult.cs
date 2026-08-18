using Authentication.Application.Common;

namespace Authentication.Application.Abstractions.Authentication;

public sealed record PasswordValidationResult
{
    public bool IsValid { get; init; }

    public Error ErrorMessage { get; init; } = new Error(
        "Validation.Password",
        "Password validation failed.",
        400);

    private PasswordValidationResult()
    {
    }

    public static PasswordValidationResult Success()
    {
        return new PasswordValidationResult
        {
            IsValid = true,
        };
    }

    public static PasswordValidationResult Failure(string message)
    {
        return new PasswordValidationResult
        {
            IsValid = false,
            ErrorMessage = new Error(
                "Validation.Password",
                message,
                400)
        };
    }
}