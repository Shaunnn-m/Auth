namespace Authentication.Domain.Entities;

public sealed class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public User User { get; private set; } = null!;

    private PasswordResetToken()
    {
    }

    private PasswordResetToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public static PasswordResetToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAt)
    {
        return new PasswordResetToken(
            userId,
            tokenHash,
            expiresAt);
    }

    public bool IsExpired(DateTime utcNow)
    {
        return utcNow >= ExpiresAt;
    }

    public bool IsUsed()
    {
        return UsedAt.HasValue;
    }

    public void MarkAsUsed(DateTime utcNow)
    {
        if (UsedAt.HasValue)
            return;

        UsedAt = utcNow;
        UpdatedAt = utcNow;
    }
}