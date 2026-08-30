using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Interfaces.Authentication
{
    public interface IAuthenticationLinkService
    {
        string GenerateEmailConfirmationLink(
            Guid userId,
            string token);

        string GeneratePasswordResetLink(
            Guid userId,
            string token);
    }
}
