using Authentication.Application.Common;

namespace Authentication.Infrastructure.Services.Common
{
    public class SecureTokenService : ISecureTokenService
    {
        public string GenerateToken()
        {
            var bytes =
                RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }

        public string HashToken(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            var bytes =
                Encoding.UTF8.GetBytes(token);

            var hash =
                SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}