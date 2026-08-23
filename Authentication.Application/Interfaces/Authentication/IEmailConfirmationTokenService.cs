using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Interfaces.Authentication
{
    public interface IEmailConfirmationTokenService
    {
        string GenerateToken();

        string HashToken(string token);

        string GenerateConfirmationLink(Guid userId, string token);
        
    }
}
