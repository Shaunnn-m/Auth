using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.GetCurrentUser
{
    public sealed record GetCurrentUserQuery : IRequest<Result<GetCurrentUserResponse>>;
}
