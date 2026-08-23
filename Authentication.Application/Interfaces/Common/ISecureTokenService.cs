using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Application.Interfaces.Common
{
    public interface ISecureTokenService
    {
        string GenerateToken();

        string HashToken(string token);
    }
}