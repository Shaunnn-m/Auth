using Authentication.Application.Errors;
using Authentication.Application.Common;
using MediatR;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Abstractions.Authentication;
using Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace Authentication.Application.Features.Authentication.Login;

public sealed class LoginUserHandler
    : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationTokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRequestContext _requestContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginUserHandler> _logger;


    public LoginUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthenticationTokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IRequestContext requestContext,
        IUnitOfWork unitOfWork,
        ILogger<LoginUserHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _requestContext = requestContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginUserResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Authentication login failed: user was not found.");
            return Result<LoginUserResponse>.Failure(
                UserErrors.UserNotFound);
        }

        if (user.Status != AccountStatus.Active)
        {
            _logger.LogWarning(
                "Authentication login failed: account is not active. AccountStatus: {AccountStatus}",
                user.Status);
            return Result<LoginUserResponse>.Failure(
                UserErrors.UserInactive);
        }

        var passwordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            _logger.LogWarning("Authentication login failed: invalid credentials.");
            return Result<LoginUserResponse>.Failure(
                AuthenticationErrors.InvalidCredentials);
        }

        var (accessToken, expiresAt) =
            _tokenService.GenerateToken(
                user.Id,
                user.Email,
                user.Role.ToString());

        var refreshToken = _refreshTokenService.GenerateToken();

        var hashedRefreshToken = _refreshTokenService.HashToken(refreshToken);

        var refreshTokenEntity = RefreshToken.Create(
            user.Id,
            hashedRefreshToken,
            DateTime.UtcNow.AddDays(30),
            Guid.NewGuid(),
            null,
            _requestContext.UserAgent,
            _requestContext.IpAddress);


        user.AddRefreshToken(refreshTokenEntity);

        await _userRepository.UpdateAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
        
        var response = new LoginUserResponse(
            accessToken,
            expiresAt,
            refreshToken);

        _logger.LogInformation("Authentication login succeeded and a new session was created.");
        return Result<LoginUserResponse>.Success(response);
    }
}