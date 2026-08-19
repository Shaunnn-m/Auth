using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Errors;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.ChangePassword
{
    public sealed class ChangePasswordHandler 
    : IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangePasswordHandler> _logger;

        public ChangePasswordHandler(IUserRepository userRepository,
            ICurrentUser currentUser, 
            IPasswordHasher passwordHasher,
            IPasswordPolicy passwordPolicy,
            IUnitOfWork unitOfWork,
            ILogger<ChangePasswordHandler> logger)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
            _passwordPolicy = passwordPolicy;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ChangePasswordResponse>> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
            {
                _logger.LogWarning("Authentication password change failed: user is not authenticated.");
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.UserNotAuthenticated);
            }

            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Authentication password change failed: user was not found.");
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.UserNotFound);
            }

            if (!_passwordHasher.Verify(user.PasswordHash, request.NewPassword))
            {
                _logger.LogWarning("Authentication password change failed: current password is invalid.");
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.InvalidCurrentPassword);
            }

            var passwordValidation =
            _passwordPolicy.Validate(request.NewPassword);

            if (!passwordValidation.IsValid)
            {
                _logger.LogWarning("Authentication password change failed: password policy validation failed.");
                return Result<ChangePasswordResponse>.Failure(
                    passwordValidation.ErrorMessage);
            }

            var newPasswordHash =
            _passwordHasher.Hash(
                request.NewPassword);

            user.ChangePassword(newPasswordHash);

            var refreshTokens =
            await _userRepository.GetActiveRefreshTokensAsync(
                user.Id,
                cancellationToken);

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoke();
            }

            await _unitOfWork.SaveChangesAsync(
            cancellationToken);

            _logger.LogInformation(
                "Authentication password change succeeded and active sessions were revoked. RevokedSessionCount: {RevokedSessionCount}",
                refreshTokens.Count);
            return Result<ChangePasswordResponse>.Success(
                new ChangePasswordResponse("Password changed successfully"));
        }
    }
}