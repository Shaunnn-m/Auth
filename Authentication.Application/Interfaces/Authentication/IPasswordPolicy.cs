namespace Authentication.Application.Interfaces.Authentication
{
    public interface IPasswordPolicy
    {
        public PasswordValidationResult Validate(string password);
    }
}