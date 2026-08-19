using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Authentication.IntegrationTests.Helpers;

public sealed record RegisterTestRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);

public sealed record LoginTestResult(
    string AccessToken,
    string RefreshToken);

public sealed record RegisterTestResult(
    RegisterTestRequest User,
    Guid UserId);

public static class AuthenticationTestHelper
{
    public static string GenerateEmail()
        => $"test-{Guid.NewGuid()}@example.com";

    public static RegisterTestRequest CreateRegistrationRequest(
        string? email = null)
    {
        return new RegisterTestRequest(
            "Integration",
            "Test",
            email ?? GenerateEmail(),
            "Password123!");
    }

    public static async Task<RegisterTestResult>
        RegisterAsync(
            HttpClient client,
            string? email = null)
    {
        var request =
            CreateRegistrationRequest(email);

        var response =
            await client.PostAsJsonAsync(
                "/api/authentication/register",
                new
                {
                    firstName = request.FirstName,
                    lastName = request.LastName,
                    email = request.Email,
                    password = request.Password
                });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<RegisterResponse>();

        if (result is null)
        {
            throw new InvalidOperationException(
                "Registration response was empty.");
        }

        return new RegisterTestResult(
            request,
            result.UserId);
    }

    public static async Task ActivateAsync(
        HttpClient client,
        Guid userId)
    {
        var response =
            await client.PostAsync(
                $"/api/authentication/activate/{userId}",
                content: null);

        response.EnsureSuccessStatusCode();
    }

    public static async Task<LoginTestResult>
        LoginAsync(
            HttpClient client,
            RegisterTestRequest user)
    {
        var response =
            await client.PostAsJsonAsync(
                "/api/authentication/login",
                new
                {
                    email = user.Email,
                    password = user.Password
                });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginTestResult>();

        return result
            ?? throw new InvalidOperationException(
                "Login response did not contain tokens.");
    }

    public static async Task<LoginTestResult>
        RegisterActivateAndLoginAsync(
            HttpClient client)
    {
        var registration =
            await RegisterAsync(client);

        await ActivateAsync(
            client,
            registration.UserId);

        return await LoginAsync(
            client,
            registration.User);
    }

    public static void AddBearerToken(
        HttpClient client,
        string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);
    }

    public static void RemoveBearerToken(
        HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }

    private sealed record RegisterResponse(
        Guid UserId);
}