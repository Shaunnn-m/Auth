namespace Authentication.Application.Abstractions.Authentication;

public interface IRefreshTokenService
{
    string GenerateToken();

    string HashToken(string token);

    bool VerifyToken(string token, string hash);
}