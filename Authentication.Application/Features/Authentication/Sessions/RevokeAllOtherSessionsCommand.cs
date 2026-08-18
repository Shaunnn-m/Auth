using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions
{
    public sealed record RevokeAllOtherSessionsCommand(
    Guid CurrentSessionId
) : IRequest<Result<RevokeSessionResponse>>;
}
