namespace Authentication.Application.Features.Authentication.Login;

public sealed record LoginUserResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken
);