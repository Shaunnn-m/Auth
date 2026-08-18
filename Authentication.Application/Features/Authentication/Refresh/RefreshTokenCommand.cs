using MediatR;
using Authentication.Application.Common;

namespace Authentication.Application.Features.Authentication.Refresh;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<RefreshTokenResponse>>;