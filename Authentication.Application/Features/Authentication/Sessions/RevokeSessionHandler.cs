using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class RevokeSessionHandler
    : IRequestHandler<RevokeSessionCommand, Result<RevokeSessionResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeSessionHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RevokeSessionResponse>> Handle(
        RevokeSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
            return Result<RevokeSessionResponse>.Failure(UserErrors.UserNotAuthenticated);
        }

        var refreshToken =
            await _userRepository.GetRefreshTokenForUserAsync(
                request.SessionId,
                userId.Value,
                cancellationToken);

        if (refreshToken is null)
        {
            return Result<RevokeSessionResponse>.Failure(
                AuthenticationErrors.SessionNotFound);
        }

        if (refreshToken.IsRevoked)
        {
            return Result<RevokeSessionResponse>.Failure(
                AuthenticationErrors.SessionAlreadyRevoked);
        }

        refreshToken.Revoke();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<RevokeSessionResponse>.Success(new RevokeSessionResponse("User session has been revoked"));
    }
}