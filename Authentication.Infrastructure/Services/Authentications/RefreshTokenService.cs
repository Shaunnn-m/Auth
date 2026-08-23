using System.Security.Cryptography;
using Authentication.Application.Interfaces.Authentication;

namespace Authentication.Infrastructure.Services.Authentications;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }

    public bool VerifyToken(
        string token,
        string hash)
    {
        var tokenHash = HashToken(token);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(tokenHash),
            Convert.FromHexString(hash));
    }
}