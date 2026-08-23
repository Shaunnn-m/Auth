

namespace Authentication.Infrastructure.Configurations.Email
{
    public sealed class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; }

        public string FromEmail { get; set; } = string.Empty;

        public string FromName { get; set; } = string.Empty;

        public string? Username { get; set; }

        public string? Password { get; set; }

        public bool UseSsl { get; set; }
    }
}
