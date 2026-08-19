using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class RevokeSessionHandler
    : IRequestHandler<RevokeSessionCommand, Result<RevokeSessionResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeSessionHandler> _logger;

    public RevokeSessionHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        ILogger<RevokeSessionHandler> logger)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RevokeSessionResponse>> Handle(
        RevokeSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
            _logger.LogWarning("Authentication session revocation failed: user is not authenticated.");
            return Result<RevokeSessionResponse>.Failure(UserErrors.UserNotAuthenticated);
        }

        var refreshToken =
            await _userRepository.GetRefreshTokenForUserAsync(
                request.SessionId,
                userId.Value,
                cancellationToken);

        if (refreshToken is null)
        {
            _logger.LogWarning("Authentication session revocation failed: session was not found.");
            return Result<RevokeSessionResponse>.Failure(
                AuthenticationErrors.SessionNotFound);
        }

        if (refreshToken.IsRevoked)
        {
            _logger.LogWarning("Authentication session revocation failed: session is already revoked.");
            return Result<RevokeSessionResponse>.Failure(
                AuthenticationErrors.SessionAlreadyRevoked);
        }

        refreshToken.Revoke();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation("Authentication session revocation succeeded.");
        return Result<RevokeSessionResponse>.Success(new RevokeSessionResponse("User session has been revoked"));
    }
}