using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class GetSessionsHandler
    : IRequestHandler<
        GetSessionsQuery,
        Result<IReadOnlyCollection<SessionResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GetSessionsHandler> _logger;

    public GetSessionsHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser,
        ILogger<GetSessionsHandler> logger)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<SessionResponse>>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
            _logger.LogWarning("Authentication session listing failed: user is not authenticated.");
            return Result<IReadOnlyCollection<SessionResponse>>.Failure(
                UserErrors.UserNotAuthenticated);
        }

        var refreshTokens =
            await _userRepository.GetRefreshTokensAsync(
                userId.Value,
                cancellationToken);

        var sessions = refreshTokens
            .Where(x => x.IsActive)
            .Select(x => new SessionResponse(
                x.Id,
                x.DeviceName,
                x.UserAgent,
                x.CreatedAt,
                x.ExpiresAt,
                false))
            .ToList();

        _logger.LogInformation(
            "Authentication session listing succeeded. ActiveSessionCount: {ActiveSessionCount}",
            sessions.Count);
        return Result<IReadOnlyCollection<SessionResponse>>
            .Success(sessions);
    }
}