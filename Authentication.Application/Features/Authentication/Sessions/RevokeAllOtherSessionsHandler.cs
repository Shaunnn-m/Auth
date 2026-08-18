using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed class RevokeAllOtherSessionsHandler
    : IRequestHandler<RevokeAllOtherSessionsCommand, Result<RevokeSessionResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeAllOtherSessionsHandler(
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RevokeSessionResponse>> Handle(
        RevokeAllOtherSessionsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
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

        return Result<RevokeSessionResponse>.Success(
            new RevokeSessionResponse("All sessions have been successfully revoked"));
    }
}