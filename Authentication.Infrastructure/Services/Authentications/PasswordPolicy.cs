using Authentication.Application.Interfaces.Authentication;
using Authentication.Infrastructure.Configurations.Authentication;
using Microsoft.Extensions.Options;

namespace Authentication.Infrastructure.Services.Authentications;

public sealed class PasswordPolicy : IPasswordPolicy
{
    private readonly PasswordOptions _options;

    public PasswordPolicy(IOptions<PasswordOptions> options)
    {
        _options = options.Value;
    }

    public PasswordValidationResult Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordValidationResult.Failure(
                "Password is required.");
        }

        if (password.Length < _options.MinimumLength)
        {
            return PasswordValidationResult.Failure(
                $"Password must be at least {_options.MinimumLength} characters.");
        }

        if (_options.RequireUppercase &&
            !password.Any(char.IsUpper))
        {
            return PasswordValidationResult.Failure(
                "Password must contain at least one uppercase letter.");
        }

        if (_options.RequireLowercase &&
            !password.Any(char.IsLower))
        {
            return PasswordValidationResult.Failure(
                "Password must contain at least one lowercase letter.");
        }

        if (_options.RequireDigit &&
            !password.Any(char.IsDigit))
        {
            return PasswordValidationResult.Failure(
                "Password must contain at least one number.");
        }

        if (_options.RequireSpecialCharacter &&
            !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            return PasswordValidationResult.Failure(
                "Password must contain at least one special character.");
        }

        return PasswordValidationResult.Success();
    }
}