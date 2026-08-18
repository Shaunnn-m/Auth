namespace Authentication.Application.Abstractions.Authentication;

public interface IAuthenticationTokenService
{
    (string AccessToken, DateTime ExpiresAt) GenerateToken(
        Guid userId,
        string email,
        string role);
}