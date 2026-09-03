using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Features.Authentication.logout;
using Authentication.Application.Errors;
using Authentication.Application.Features.Authentication.Refresh;
using Microsoft.Extensions.Logging;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Application.Interfaces.Persistence;

namespace Authentication.Application.Features.Authentication.Logout;

public sealed class LogoutHandler
    : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutHandler> _logger;

    public LogoutHandler(
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IUnitOfWork unitOfWork,
        ILogger<LogoutHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LogoutResponse>> Handle(
        LogoutCommand request,
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
            _logger.LogWarning("Authentication logout failed: refresh token is invalid.");
            return Result<LogoutResponse>.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        if (storedToken.IsRevoked)
        {
            _logger.LogWarning("Authentication logout failed: refresh token is already revoked.");
            return Result<LogoutResponse>.Failure(AuthenticationErrors.TokenRevoked);
        }

        storedToken.Revoke();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation("Authentication logout succeeded and the session was revoked.");
        return Result<LogoutResponse>.Success(
            new LogoutResponse("User has been successfully logged out.")
            );
    }
}