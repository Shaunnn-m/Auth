using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Domain.Entities;
using Authentication.Application.Common;
using MediatR;
using Authentication.Application.Errors;

namespace Authentication.Application.Features.Authentication.Register;

public sealed class RegisterUserHandler
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordPolicy passwordPolicy,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _unitOfWork = unitOfWork;
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
            return Result<RegisterUserResponse>.Failure(
               UserErrors.EmailAlreadyExists);
        }

        var passwordValidation =
            _passwordPolicy.Validate(request.Password);

        if (!passwordValidation.IsValid)
        {
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

        return  Result<RegisterUserResponse>.Success(
            new RegisterUserResponse(
            "User registered successfully."));
    }
}