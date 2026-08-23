namespace Authentication.Domain.Entities;

public sealed class EmailConfirmationToken : BaseEntity
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public User User { get; private set; } = null!;

    private EmailConfirmationToken()
    {
    }

    private EmailConfirmationToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public static EmailConfirmationToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAt)
    {
        return new EmailConfirmationToken(
            userId,
            tokenHash,
            expiresAt);
    }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsUsed =>
        UsedAt.HasValue;

    public bool IsValid =>
        !IsUsed && !IsExpired;

    public void MarkAsUsed()
    {
        if (IsUsed)
        {
            return;
        }

        UsedAt = DateTime.UtcNow;
    }
}