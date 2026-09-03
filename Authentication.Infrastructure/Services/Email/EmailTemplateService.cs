using System.Reflection;

namespace Authentication.Infrastructure.Services.Email;

public sealed class EmailTemplateService
{
    public string RenderConfirmationEmail(
        string confirmationLink)
    {
        var assembly =
            Assembly.GetExecutingAssembly();

        const string resourceName =
            "Authentication.Infrastructure.Templates.Email.ConfirmEmail.html";

        using var stream =
            assembly.GetManifestResourceStream(
                resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Email template '{resourceName}' was not found.");
        }

        using var reader =
            new StreamReader(stream);

        var template =
            reader.ReadToEnd();

        return template.Replace(
            "{{CONFIRMATION_LINK}}",
            confirmationLink);
    }

    public string RenderPasswordResetEmail(
        string resetLink)
    {
        var assembly =
            Assembly.GetExecutingAssembly();

        const string resourceName =
            "Authentication.Infrastructure.Templates.Email.PasswordReset.html";

        using var stream =
            assembly.GetManifestResourceStream(
                resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Email template '{resourceName}' was not found.");
        }

        using var reader =
            new StreamReader(stream);

        var template =
            reader.ReadToEnd();

        return template.Replace(
            "{{RESET_LINK}}",
            resetLink);
    }
}