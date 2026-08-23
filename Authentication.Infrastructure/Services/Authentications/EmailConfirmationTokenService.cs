using System.Security.Cryptography;
using System.Text;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Authentication.Infrastructure.Services.Authentications;

public sealed class EmailConfirmationTokenService
    : IEmailConfirmationTokenService
{
    IOptions<ApplicationOptions> _applicationOptions;

    public EmailConfirmationTokenService(IOptions<ApplicationOptions> applicationOptions)
    {
        _applicationOptions = applicationOptions;
    }

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


    public string GenerateConfirmationLink(Guid userId, string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        var baseUrl = _applicationOptions.Value.BaseUrl.Trim();
        return $"{baseUrl}/api/auth/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

}