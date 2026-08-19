using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Domain.Entities;
using Authentication.Application.Common;
using MediatR;
using Authentication.Application.Errors;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentication.Register;

public sealed class RegisterUserHandler
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordPolicy passwordPolicy,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning("Authentication registration failed: email is already registered.");
            return Result<RegisterUserResponse>.Failure(
               UserErrors.EmailAlreadyExists);
        }

        var passwordValidation =
            _passwordPolicy.Validate(request.Password);

        if (!passwordValidation.IsValid)
        {
            _logger.LogWarning("Authentication registration failed: password policy validation failed.");
            return Result<RegisterUserResponse>.Failure(
                passwordValidation.ErrorMessage);
        }

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation("Authentication registration succeeded.");
        return  Result<RegisterUserResponse>.Success(
            new RegisterUserResponse(
            user.Id,
            "User registered successfully."));
    }
}