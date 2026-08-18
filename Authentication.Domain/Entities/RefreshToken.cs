namespace Authentication.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAt,
        Guid tokenFamilyId,
        string? deviceName = null,
        string? userAgent = null,
        string? ipAddress = null)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        TokenFamilyId = tokenFamilyId;
        DeviceName = deviceName;
        UserAgent = userAgent;
        IpAddress = ipAddress;
    }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; }

    public string? DeviceName { get; private set; }

    public Guid TokenFamilyId { get; private set; }

    public string? UserAgent { get; private set; }

    public string? IpAddress { get; private set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsActive =>
        !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAt,
        Guid tokenFamilyId,
        string? deviceName = null,
        string? userAgent = null,
        string? ipAddress = null)
    {
        return new RefreshToken(
            userId,
            tokenHash,
            expiresAt,
            tokenFamilyId,
            deviceName,
            userAgent,
            ipAddress);
    }
}