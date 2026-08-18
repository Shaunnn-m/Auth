using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using Authentication.Application.Features.Authentication.Login;
using MediatR;

namespace Authentication.Application.Features.Authentication.ActivateUser
{
    public sealed class ActivateUserHandler
    : IRequestHandler<ActivateUserCommand, Result<ActivateUserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
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
                return Result<ActivateUserResponse>.Failure(
                UserErrors.UserNotFound);
            }

            if (user.Status == AccountStatus.Active)
            {
                return Result<ActivateUserResponse>.Failure(
                UserErrors.UserAlreadyActive);
            }

            user.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ActivateUserResponse>.Success(
                new ActivateUserResponse("User activated successfully.")
                );
        }
    }
}
