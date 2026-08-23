using Authentication.Application.Abstractions.Persistence;
using Authentication.Application.Common;
using Authentication.Application.Errors;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Application.Interfaces.Common;
using Authentication.Application.Persistence;
using Authentication.Domain.Entities;
using Authentication.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication
    .ResendEmailConfirmation;

public sealed class ResendEmailConfirmationHandler
    : IRequestHandler<
        ResendEmailConfirmationCommand,
        Result<ResendEmailConfirmationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailConfirmationTokenRepository
        _emailConfirmationTokenRepository;
    private readonly IEmailConfirmationTokenService
        _emailConfirmationTokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResendEmailConfirmationHandler>
        _logger;

    public ResendEmailConfirmationHandler(
        IUserRepository userRepository,
        IEmailConfirmationTokenRepository
            emailConfirmationTokenRepository,
        IEmailConfirmationTokenService
            emailConfirmationTokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<ResendEmailConfirmationHandler> logger)
    {
        _userRepository = userRepository;
        _emailConfirmationTokenRepository =
            emailConfirmationTokenRepository;
        _emailConfirmationTokenService =
            emailConfirmationTokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ResendEmailConfirmationResponse>> Handle(
        ResendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "Email confirmation resend requested for non-existent email {Email}",
                request.Email);

            return Result<ResendEmailConfirmationResponse>.Failure(
                UserErrors.UserNotFound);
        }

        if (user.Status != AccountStatus.PendingVerification)
        {
            _logger.LogWarning(
                "Email confirmation resend requested for active user {UserId}",
                user.Id);

            return Result<ResendEmailConfirmationResponse>.Failure(
                UserErrors.UserAlreadyActive);
        }

        var existingTokens =
            await _emailConfirmationTokenRepository
                .GetUnusedByUserIdAsync(
                    user.Id,
                    cancellationToken);

        foreach (var token in existingTokens)
        {
            token.MarkAsUsed();
        }

        var rawToken =
            _emailConfirmationTokenService
                .GenerateToken();

        var tokenHash =
            _emailConfirmationTokenService
                .HashToken(rawToken);

        var confirmationToken =
            EmailConfirmationToken.Create(
                user.Id,
                tokenHash,
                DateTime.UtcNow.AddHours(24));

        await _emailConfirmationTokenRepository
            .AddAsync(
                confirmationToken,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        var confirmationLink =
            _emailConfirmationTokenService
                .GenerateConfirmationLink(
                    user.Id,
                    rawToken);

        await _emailService.SendEmailConfirmationAsync(
            user.Email,
            confirmationLink,
            cancellationToken);

        _logger.LogInformation(
            "Email confirmation resent for user {UserId}",
            user.Id);

        return Result<ResendEmailConfirmationResponse>.Success(
            new ResendEmailConfirmationResponse(
                "Email confirmation has been resent."));
    }
}