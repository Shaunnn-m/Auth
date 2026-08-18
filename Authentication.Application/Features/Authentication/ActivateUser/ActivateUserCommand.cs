using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.ActivateUser;

public sealed record ActivateUserCommand(
    Guid UserId
) : IRequest<Result<ActivateUserResponse>>;