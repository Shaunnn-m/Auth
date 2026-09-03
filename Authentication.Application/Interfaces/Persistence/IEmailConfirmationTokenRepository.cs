using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces.Persistence
{
    public interface IEmailConfirmationTokenRepository
    {
        Task AddAsync(
            EmailConfirmationToken token,
            CancellationToken cancellationToken);

        Task<EmailConfirmationToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<EmailConfirmationToken>> GetUnusedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }
}
