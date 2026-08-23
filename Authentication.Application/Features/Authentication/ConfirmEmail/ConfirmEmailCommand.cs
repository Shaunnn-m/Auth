using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.ConfirmEmail
{
    public sealed record ConfirmEmailCommand(
    Guid UserId,
    string Token) : IRequest<Result<ConfirmEmailResponse>>;
}
