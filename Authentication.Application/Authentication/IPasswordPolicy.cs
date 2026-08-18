
namespace Authentication.Application.Abstractions.Authentication
{
    public interface IPasswordPolicy
    {
        public PasswordValidationResult Validate(string password);
    }
}