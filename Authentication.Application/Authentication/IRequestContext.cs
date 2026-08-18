namespace Authentication.Application.Abstractions.Authentication;

public interface IRequestContext
{
    string? UserAgent { get; }

    string? IpAddress { get; }
}