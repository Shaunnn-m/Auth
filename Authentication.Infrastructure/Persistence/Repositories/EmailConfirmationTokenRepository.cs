using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Authentication.Domain.Entities;
using Authentication.Application.Interfaces.Persistence;

namespace Authentication.Infrastructure.Persistence.Repositories
{
    public sealed class EmailConfirmationTokenRepository : IEmailConfirmationTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public EmailConfirmationTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(
            EmailConfirmationToken token,
            CancellationToken cancellationToken)
        {
            await _dbContext.EmailConfirmationTokens.AddAsync(token, cancellationToken);
        }
        public async Task<EmailConfirmationToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken)
        {
            return await _dbContext.EmailConfirmationTokens
                .Include(token => token.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<IReadOnlyList<EmailConfirmationToken>> GetUnusedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.EmailConfirmationTokens
                .Where(token => token.UserId == userId && !token.IsUsed)
                .ToListAsync(cancellationToken);
        }
    }
}
