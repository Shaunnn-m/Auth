namespace Authentication.Application.Interfaces.Authentication;

public interface IRequestContext
{
    string? UserAgent { get; }

    string? IpAddress { get; }
}