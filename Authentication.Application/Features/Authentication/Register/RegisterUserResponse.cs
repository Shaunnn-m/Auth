namespace Authentication.Application.Features.Authentication.Register;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Message
);