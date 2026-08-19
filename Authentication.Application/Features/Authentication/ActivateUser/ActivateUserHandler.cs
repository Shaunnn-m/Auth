using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using Authentication.Application.Features.Authentication.Login;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.ActivateUser
{
    public sealed class ActivateUserHandler
    : IRequestHandler<ActivateUserCommand, Result<ActivateUserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivateUserHandler> _logger;

        public ActivateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<ActivateUserHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ActivateUserResponse>> Handle(
            ActivateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Authentication activation failed: user was not found.");
                return Result<ActivateUserResponse>.Failure(
                UserErrors.UserNotFound);
            }

            if (user.Status == AccountStatus.Active)
            {
                _logger.LogWarning("Authentication activation failed: account is already active.");
                return Result<ActivateUserResponse>.Failure(
                UserErrors.UserAlreadyActive);
            }

            user.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Authentication activation succeeded.");
            return Result<ActivateUserResponse>.Success(
                new ActivateUserResponse("User activated successfully.")
                );
        }
    }
}
