namespace Authentication.Application.Interfaces.Authentication
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }

        string? Email { get; }

        bool IsAuthenticated { get; }
    }
}
