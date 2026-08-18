using Authentication.Application.Abstractions.Authentication;
using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Errors;
using Authentication.Domain.Entities;


namespace Authentication.Application.Features.Authentication.Refresh;

public sealed class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthenticationTokenService _authenticationTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IAuthenticationTokenService authenticationTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _authenticationTokenService = authenticationTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _refreshTokenService.HashToken(
                request.RefreshToken);

        var storedToken =
            await _userRepository.GetRefreshTokenAsync(
                tokenHash,
                cancellationToken);

        if (storedToken is null)
        {
            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        if (storedToken.IsRevoked)
        {
            var activeTokens =
                await _userRepository
                    .GetActiveRefreshTokensByFamilyAsync(
                        storedToken.TokenFamilyId,
                        cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(
                AuthenticationErrors.SessionReuseDetected);
        }

        if (storedToken.IsExpired)
        {
            return Result<RefreshTokenResponse>.Failure(
                AuthenticationErrors.TokenExpired);
        }

        var user =
            await _userRepository.GetByIdAsync(
                storedToken.UserId,
                cancellationToken);

        if (user is null)
        {
            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        if (user.Status != AccountStatus.Active)
        {
            return Result<RefreshTokenResponse>.Failure(UserErrors.UserInactive);
        }

        storedToken.Revoke();

        var newRefreshToken =
            _refreshTokenService.GenerateToken();

        var newRefreshTokenHash =
            _refreshTokenService.HashToken(
                newRefreshToken);

        var newRefreshTokenEntity =
            RefreshToken.Create(
                user.Id,
                newRefreshTokenHash,
                DateTime.UtcNow.AddDays(30),
                storedToken.TokenFamilyId,
                storedToken.DeviceName,
                storedToken.UserAgent,
                storedToken.IpAddress);

        user.AddRefreshToken(
            newRefreshTokenEntity);

        var accessToken =
            _authenticationTokenService.GenerateToken(
                user.Id,
                user.Email,
                user.Role.ToString());

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(
                accessToken.AccessToken,
                newRefreshToken));
    }
}