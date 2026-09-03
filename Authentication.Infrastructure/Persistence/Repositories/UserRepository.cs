using Authentication.Application.Interfaces.Persistence;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(
    string tokenHash,
    CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>>
    GetRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenForUserAsync(
    Guid refreshTokenId,
    Guid userId,
    CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Id == refreshTokenId &&
                     x.UserId == userId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>>
    GetActiveRefreshTokensExceptAsync(
        Guid userId,
        Guid currentSessionId,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.Id != currentSessionId &&
                !x.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>>
    GetActiveRefreshTokensByFamilyAsync(
        Guid tokenFamilyId,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(x =>
                x.TokenFamilyId == tokenFamilyId &&
                !x.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>>
    GetActiveRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}