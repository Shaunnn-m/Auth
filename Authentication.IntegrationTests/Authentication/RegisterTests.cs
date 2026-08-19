using System.Net;
using System.Net.Http.Json;
using Authentication.IntegrationTests.Infrastructure;
using Authentication.IntegrationTests.Helpers;

namespace Authentication.IntegrationTests.Authentication;

public sealed class RegisterTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegisterTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var request =
            AuthenticationTestHelper
                .CreateRegistrationRequest();

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/register",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var request =
            AuthenticationTestHelper
                .CreateRegistrationRequest();

        // Act
        var first =
            await _client.PostAsJsonAsync(
                "/api/authentication/register",
                request);

        first.EnsureSuccessStatusCode();

        var second =
            await _client.PostAsJsonAsync(
                "/api/authentication/register",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Conflict,
            second.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            firstName = "Integration",
            lastName = "Test",
            email = "not-an-email",
            password = "Password123!"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/register",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            firstName = "Integration",
            lastName = "Test",
            email =
                AuthenticationTestHelper.GenerateEmail(),
            password = "123"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/authentication/register",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}