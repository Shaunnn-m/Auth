using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions;

public sealed record RevokeSessionCommand(
    Guid SessionId
) : IRequest<Result<RevokeSessionResponse>>;