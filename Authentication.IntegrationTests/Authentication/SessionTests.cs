using System.Net;
using System.Net.Http.Json;
using Authentication.IntegrationTests.Helpers;
using Authentication.IntegrationTests.Infrastructure;

namespace Authentication.IntegrationTests.Authentication;

public sealed class SessionTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SessionTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RevokeSession_RefreshTokenCanNoLongerBeUsed()
    {
        // Arrange
        var tokens =
            await AuthenticationTestHelper
                .RegisterActivateAndLoginAsync(_client);

        AuthenticationTestHelper.AddBearerToken(
            _client,
            tokens.AccessToken);

        var sessionsResponse =
            await _client.GetAsync(
                "/api/authentication/sessions");

        sessionsResponse.EnsureSuccessStatusCode();

        var sessions =
            await sessionsResponse.Content
                .ReadFromJsonAsync<SessionResponse[]>();

        Assert.NotNull(sessions);
        Assert.Single(sessions);

        // Act - revoke the session
        var revoke =
            await _client.DeleteAsync(
                $"/api/authentication/sessions/{sessions[0].Id}");

        // Assert - revoke succeeded
        Assert.True(
            revoke.IsSuccessStatusCode);

        // Act - attempt to use revoked refresh token
        var refresh =
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
            refresh.StatusCode);
    }

    private sealed record SessionResponse(
        Guid Id);
}