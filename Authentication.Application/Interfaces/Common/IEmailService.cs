

namespace Authentication.Application.Interfaces.Common
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(
            string email,
            string confirmationLink,
            CancellationToken cancellationToken);
    }
}
