
namespace Authentication.Application.Interfaces.Common
{
    public interface ISecureTokenService
    {
        string GenerateToken();

        string HashToken(string token);
    }
}