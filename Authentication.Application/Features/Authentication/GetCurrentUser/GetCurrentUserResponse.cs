namespace Authentication.Application.Features.Authentication.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsActive);