using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class GetSessionsHandler
    : IRequestHandler<
        GetSessionsQuery,
        Result<IReadOnlyCollection<SessionResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;

    public GetSessionsHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyCollection<SessionResponse>>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
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

        return Result<IReadOnlyCollection<SessionResponse>>
            .Success(sessions);
    }
}