using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces.Persistence;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(
        PasswordResetToken passwordResetToken,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PasswordResetToken>>
        GetUnusedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);
}