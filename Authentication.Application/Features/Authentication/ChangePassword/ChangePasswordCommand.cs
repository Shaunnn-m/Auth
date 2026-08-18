using Authentication.Application.Common;
using MediatR;

namespace Authentication.Application.Features.Authentication.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : IRequest<Result<ChangePasswordResponse>>;
