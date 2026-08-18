namespace Authentication.Infrastructure.Configurations.Authentication
{
    public sealed class PasswordOptions
    {
        public const string SectionName = "PasswordPolicy";

        public int MinimumLength { get; init; }

        public bool RequireUppercase { get; init; }

        public bool RequireLowercase { get; init; }

        public bool RequireDigit { get; init; }

        public bool RequireSpecialCharacter { get; init; }
    }
}
