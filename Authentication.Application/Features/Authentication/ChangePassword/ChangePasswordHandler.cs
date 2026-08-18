using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Errors;

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

        public ChangePasswordHandler(IUserRepository userRepository,
            ICurrentUser currentUser, 
            IPasswordHasher passwordHasher,
            IPasswordPolicy passwordPolicy,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
            _passwordPolicy = passwordPolicy;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ChangePasswordResponse>> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
            {
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.UserNotAuthenticated);
            }

            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

            if (user is null)
            {
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.UserNotFound);
            }

            if (!_passwordHasher.Verify(user.PasswordHash, request.NewPassword))
            {
                return Result<ChangePasswordResponse>.Failure(
                    UserErrors.InvalidCurrentPassword);
            }

            var passwordValidation =
            _passwordPolicy.Validate(request.NewPassword);

            if (!passwordValidation.IsValid)
            {
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

            return Result<ChangePasswordResponse>.Success(
                new ChangePasswordResponse("Password changed successfully"));
        }
    }
}