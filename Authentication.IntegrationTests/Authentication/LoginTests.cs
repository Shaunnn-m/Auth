using System.Net;
using System.Net.Http.Json;
using Authentication.IntegrationTests.Helpers;
using Authentication.IntegrationTests.Infrastructure;

namespace Authentication.IntegrationTests.Authentication;

public sealed class LoginTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var registration =
            await AuthenticationTestHelper.RegisterAsync(
                _client);

        await AuthenticationTestHelper.ActivateAsync(
            _client,
            registration.UserId);

        // Act
        var tokens =
            await AuthenticationTestHelper.LoginAsync(
                _client,
                registration.User);

        // Assert
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var registration =
            await AuthenticationTestHelper.RegisterAsync(
                _client);

        await AuthenticationTestHelper.ActivateAsync(
            _client,
            registration.UserId);

        var loginRequest = new
        {
            email = registration.User.Email,
            password = "WrongPassword123!"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/login",
                loginRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}