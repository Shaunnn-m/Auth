using MediatR;
using Authentication.Application.Common;

namespace Authentication.Application.Features.Authentication.Register;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<RegisterUserResponse>>;