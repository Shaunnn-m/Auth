namespace Authentication.Application.Common
{
    public sealed record Error(
        string Code,
        string Message,
        int? StatusCode = null);
}
