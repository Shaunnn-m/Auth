using Authentication.Application.Common;
using Authentication.Application.Errors;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.GetCurrentUser;

public sealed class GetCurrentUserHandler
    : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetCurrentUserHandler> _logger;

    public GetCurrentUserHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        ILogger<GetCurrentUserHandler> logger)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<GetCurrentUserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == null)
        {
            _logger.LogWarning("Authentication current-user lookup failed: user is not authenticated.");
            return Result<GetCurrentUserResponse>.Failure(
                UserErrors.UserNotAuthenticated);
        }

        var user = await _userRepository.GetByIdAsync(
            userId.Value,
            cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Authentication current-user lookup failed: user was not found.");
            return Result<GetCurrentUserResponse>.Failure(
                UserErrors.UserNotFound);
        }

        var response = new GetCurrentUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString(),
            user.Status == AccountStatus.Active);

        _logger.LogInformation("Authentication current-user lookup succeeded.");
        return Result<GetCurrentUserResponse>.Success(response);
    }

}