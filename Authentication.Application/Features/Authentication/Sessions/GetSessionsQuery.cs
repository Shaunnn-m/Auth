using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.Sessions
{
    public sealed record GetSessionsQuery
     : IRequest<Result<IReadOnlyCollection<SessionResponse>>>;
}
