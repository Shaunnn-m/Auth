using System.Net;
using System.Net.Http.Json;
using Authentication.IntegrationTests.Helpers;
using Authentication.IntegrationTests.Infrastructure;

namespace Authentication.IntegrationTests.Authentication;

public sealed class RefreshTokenTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RefreshTokenTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var tokens =
            await AuthenticationTestHelper
                .RegisterActivateAndLoginAsync(_client);

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/refresh",
                new
                {
                    refreshToken = tokens.RefreshToken
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var refreshed =
            await response.Content
                .ReadFromJsonAsync<LoginTestResult>();

        Assert.NotNull(refreshed);

        Assert.NotEqual(
            tokens.RefreshToken,
            refreshed.RefreshToken);

        Assert.False(
            string.IsNullOrWhiteSpace(
                refreshed.AccessToken));

        Assert.False(
            string.IsNullOrWhiteSpace(
                refreshed.RefreshToken));
    }

    [Fact]
    public async Task Refresh_InvalidToken_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/refresh",
                new
                {
                    refreshToken =
                        "invalid-refresh-token"
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Refresh_UsedToken_IsRejected()
    {
        // Arrange
        var tokens =
            await AuthenticationTestHelper
                .RegisterActivateAndLoginAsync(_client);

        // Act - first refresh should succeed
        var first =
            await _client.PostAsJsonAsync(
                "/api/authentication/refresh",
                new
                {
                    refreshToken =
                        tokens.RefreshToken
                });

        first.EnsureSuccessStatusCode();

        // Act - reuse the original refresh token
        var second =
            await _client.PostAsJsonAsync(
                "/api/authentication/refresh",
                new
                {
                    refreshToken =
                        tokens.RefreshToken
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            second.StatusCode);
    }
}