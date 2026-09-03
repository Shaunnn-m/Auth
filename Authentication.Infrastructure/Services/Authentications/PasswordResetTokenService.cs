using Authentication.Application.Interfaces.Persistence;
using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Data.Repositories;

public sealed class PasswordResetTokenRepository
    : IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PasswordResetToken passwordResetToken,
        CancellationToken cancellationToken)
    {
        await _context.PasswordResetTokens.AddAsync(
            passwordResetToken,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PasswordResetToken>>
        GetUnusedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
    {
        return await _context.PasswordResetTokens
            .Where(x =>
                x.UserId == userId &&
                x.UsedAt == null)
            .ToListAsync(cancellationToken);
    }
}