using MediatR;
using Authentication.Application.Common;
using Authentication.Application.Features.Authentication.logout;

namespace Authentication.Application.Features.Authentication.Logout;

public sealed record LogoutCommand(
    string RefreshToken
) : IRequest<Result<LogoutResponse>>;