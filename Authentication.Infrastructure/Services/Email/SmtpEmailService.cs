using Authentication.Application.Interfaces.Common;
using Authentication.Infrastructure.Configurations.Email;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Authentication.Infrastructure.Services.Email
{
    class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly EmailTemplateService _templateService;

        public SmtpEmailService(IOptions<SmtpOptions> options, EmailTemplateService templateService)
        {
            _options = options.Value;
            _templateService = templateService;
        }

        public async Task SendEmailConfirmationAsync(
            string email,
            string confirmationLink,
            CancellationToken cancellationToken)
        {

            var body = _templateService.RenderConfirmationEmail(
               confirmationLink);

            using var message = new MailMessage
            {
                From = new MailAddress(
                _options.FromEmail,
                _options.FromName),
                Subject = "Confirm your account",
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var client = new SmtpClient(
            _options.Host,
            _options.Port)
            {
                EnableSsl = _options.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                client.Credentials =
                    new NetworkCredential(
                        _options.Username,
                        _options.Password);
            }
            else
            {
                client.UseDefaultCredentials = false;
            }

            cancellationToken.ThrowIfCancellationRequested();

            await client.SendMailAsync(message, cancellationToken);
        }

        public async Task SendPasswordResetEmailAsync(
            string email,
            string resetLink,
            CancellationToken cancellationToken)
        {
            var body = _templateService.RenderPasswordResetEmail(
                resetLink);

            using var message = new MailMessage
            {
                From = new MailAddress(
                    _options.FromEmail,
                    _options.FromName),
                Subject = "Reset your password",
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var client = new SmtpClient(
                _options.Host,
                _options.Port)
            {
                EnableSsl = _options.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                client.Credentials =
                    new NetworkCredential(
                        _options.Username,
                        _options.Password);
            }
            else
            {
                client.UseDefaultCredentials = false;
            }

            cancellationToken.ThrowIfCancellationRequested();

            await client.SendMailAsync(message, cancellationToken);
        }

    }
}
