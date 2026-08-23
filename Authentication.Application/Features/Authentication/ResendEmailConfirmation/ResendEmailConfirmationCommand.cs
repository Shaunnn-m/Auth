using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication
    .ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(
    string Email)
    : IRequest<Result<ResendEmailConfirmationResponse>>;