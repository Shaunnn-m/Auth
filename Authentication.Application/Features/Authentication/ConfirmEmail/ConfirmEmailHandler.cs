using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Application.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Authentication.Application.Interfaces.Common;

namespace Authentication.Application.Features.Authentication.ConfirmEmail;

public sealed class ConfirmEmailHandler
    : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
{
    private readonly IEmailConfirmationTokenService
        _tokenService;

    private readonly IEmailConfirmationTokenRepository
        _tokenRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly ILogger<ConfirmEmailHandler>
        _logger;

    private readonly ISecureTokenService
        _secureTokenService;

    public ConfirmEmailHandler(
        IEmailConfirmationTokenService tokenService,
        IEmailConfirmationTokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        ISecureTokenService secureTokenService,
        ILogger<ConfirmEmailHandler> logger)
    {
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _secureTokenService = secureTokenService;
        _logger = logger;
    }

    public async Task<Result<ConfirmEmailResponse>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _secureTokenService.HashToken(
                request.Token);

        var confirmationToken =
            await _tokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (confirmationToken is null ||
            confirmationToken.UserId != request.UserId)
        {
            _logger.LogWarning(
                "Email confirmation failed: token was invalid.");
            return Result<ConfirmEmailResponse>.Failure(
                AuthenticationErrors.InvalidEmailConfirmationToken);
        }

        if (confirmationToken.IsUsed)
        {
            _logger.LogWarning(
                "Email confirmation failed: token was already used.");
            return Result<ConfirmEmailResponse>.Failure(
                AuthenticationErrors.EmailConfirmationTokenUsed);
        }

        if (confirmationToken.IsExpired)
        {
            _logger.LogWarning(
                "Email confirmation failed: token was expired.");
            return Result<ConfirmEmailResponse>.Failure(
                AuthenticationErrors.EmailConfirmationTokenExpired);
        }

        if (confirmationToken.User.Status != AccountStatus.PendingVerification)
        {
            _logger.LogWarning(
                "Email confirmation failed: account was already verified or is unavailable. AccountStatus: {AccountStatus}",
                confirmationToken.User.Status);
            return Result<ConfirmEmailResponse>.Failure(
                AuthenticationErrors.EmailAlreadyConfirmed);
        }

        confirmationToken.User.VerifyEmail();
        confirmationToken.MarkAsUsed();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Email confirmation succeeded for user {UserId}.",
            request.UserId);

        return Result<ConfirmEmailResponse>.Success(
            new ConfirmEmailResponse(
                "Email confirmed successfully."));
    }
}