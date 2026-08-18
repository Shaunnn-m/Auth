using MediatR;
using Authentication.Application.Common;

namespace Authentication.Application.Features.Authentication.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password
) : IRequest<Result<LoginUserResponse>>;