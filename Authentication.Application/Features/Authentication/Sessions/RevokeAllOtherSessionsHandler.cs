using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class RevokeAllOtherSessionsHandler
    : IRequestHandler<RevokeAllOtherSessionsCommand, Result<RevokeSessionResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeAllOtherSessionsHandler> _logger;

    public RevokeAllOtherSessionsHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        ILogger<RevokeAllOtherSessionsHandler> logger)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RevokeSessionResponse>> Handle(
        RevokeAllOtherSessionsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
            _logger.LogWarning("Authentication bulk session revocation failed: user is not authenticated.");
            return Result<RevokeSessionResponse>.Failure(
                UserErrors.UserNotAuthenticated);
        }

        var sessions =
            await _userRepository.GetActiveRefreshTokensExceptAsync(
                userId.Value,
                request.CurrentSessionId,
                cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Authentication bulk session revocation succeeded. RevokedSessionCount: {RevokedSessionCount}",
            sessions.Count);
        return Result<RevokeSessionResponse>.Success(
            new RevokeSessionResponse("All sessions have been successfully revoked"));
    }
}