using Authentication.Domain.Entities;

namespace Authentication.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RefreshToken>> GetRefreshTokensAsync(
    Guid userId,
    CancellationToken cancellationToken);

    Task<RefreshToken?> GetRefreshTokenForUserAsync(
    Guid refreshTokenId,
    Guid userId,
    CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RefreshToken>> GetActiveRefreshTokensExceptAsync(
    Guid userId,
    Guid currentSessionId,
    CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RefreshToken>>
    GetActiveRefreshTokensByFamilyAsync(
        Guid tokenFamilyId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RefreshToken>> GetActiveRefreshTokensAsync(
    Guid userId,
    CancellationToken cancellationToken);
}