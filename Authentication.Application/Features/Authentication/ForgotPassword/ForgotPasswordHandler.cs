
using Authentication.Application.Common;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Application.Interfaces.Common;
using Authentication.Application.Interfaces.Persistence;
using Authentication.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.ForgotPassword;

public sealed class ForgotPasswordHandler
    : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly ISecureTokenService _secureTokenService;
    private readonly IAuthenticationLinkService _linkService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        ISecureTokenService secureTokenService,
        IAuthenticationLinkService linkService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<ForgotPasswordHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _secureTokenService = secureTokenService;
        _linkService = linkService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        // Don't reveal whether an account exists.
        if (user is null)
        {
            _logger.LogInformation(
                "Password reset requested for an unknown email.");

            return Result<ForgotPasswordResponse>.Success(
                new ForgotPasswordResponse(
                    "If an account exists for this email, a password reset email has been sent."));
        }

        var existingTokens =
            await _tokenRepository.GetUnusedByUserIdAsync(
                user.Id,
                cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var token in existingTokens)
        {
            token.MarkAsUsed(now);
        }

        var rawToken =
            _secureTokenService.GenerateToken();

        var tokenHash =
            _secureTokenService.HashToken(rawToken);

        var passwordResetToken =
            PasswordResetToken.Create(
                user.Id,
                tokenHash,
                now.AddHours(1));

        await _tokenRepository.AddAsync(
            passwordResetToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        var resetLink =
            _linkService.GeneratePasswordResetLink(
                user.Id,
                rawToken);

        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            resetLink,
            cancellationToken);

        _logger.LogInformation(
            "Password reset email sent for user {UserId}",
            user.Id);

        return Result<ForgotPasswordResponse>.Success(
            new ForgotPasswordResponse(
                "If an account exists for this email, a password reset email has been sent."));
    }
}