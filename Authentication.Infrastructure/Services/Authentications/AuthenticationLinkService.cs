using Authentication.Application.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using Authentication.Infrastructure.Configurations;

namespace Authentication.Infrastructure.Services.Authentications
{
    public sealed class AuthenticationLinkService
    : IAuthenticationLinkService
    {
        private readonly ApplicationOptions _options;

        public AuthenticationLinkService(
            IOptions<ApplicationOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateEmailConfirmationLink(
            Guid userId,
            string token)
        {
            return
                $"{_options.BaseUrl.TrimEnd('/')}" +
                $"/api/authentication/confirm-email" +
                $"?userId={userId}" +
                $"&token={Uri.EscapeDataString(token)}";
        }

        public string GeneratePasswordResetLink(
            Guid userId,
            string token)
        {
            return
                $"{_options.BaseUrl.TrimEnd('/')}" +
                $"/api/authentication/reset-password" +
                $"?userId={userId}" +
                $"&token={Uri.EscapeDataString(token)}";
        }
    }
}
