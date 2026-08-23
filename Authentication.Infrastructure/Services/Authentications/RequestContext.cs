using Authentication.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace Authentication.Infrastructure.Services.Authentications
{
    public class RequestContext : IRequestContext
    {
        private readonly HttpContext _httpContext;

        public RequestContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));
        }

        public string? UserAgent => _httpContext
            .Request
            .Headers["User-Agent"]
            .ToString();

        public string? IpAddress => _httpContext.Connection.RemoteIpAddress?.ToString();
    }
}