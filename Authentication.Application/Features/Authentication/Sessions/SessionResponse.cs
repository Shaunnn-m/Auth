namespace Authentication.Application.Features.Authentication.Sessions
{
    public sealed record SessionResponse(
    Guid Id,
    string? DeviceName,
    string? UserAgent,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsCurrent);
}
