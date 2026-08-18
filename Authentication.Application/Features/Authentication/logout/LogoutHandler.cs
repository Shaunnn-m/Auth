using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Features.Authentication.logout;
using Authentication.Application.Errors;
using Authentication.Application.Features.Authentication.Refresh;

namespace Authentication.Application.Features.Authentication.Logout;

public sealed class LogoutHandler
    : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutHandler(
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
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
            return Result<LogoutResponse>.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        if (storedToken.IsRevoked)
        {
            return Result<LogoutResponse>.Failure(AuthenticationErrors.TokenRevoked);
        }

        storedToken.Revoke();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<LogoutResponse>.Success(
            new LogoutResponse("User has been successfully logged out.")
            );
    }
}