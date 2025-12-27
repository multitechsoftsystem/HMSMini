using System.Net.Http.Headers;
using System.Net.Http.Json;
using HMSMini.API.Models.DTOs.Auth;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public static class TestAuthHelper
{
    public static async Task<string> GetAuthTokenAsync(HttpClient client, UserRole role = UserRole.Receptionist)
    {
        var username = $"testuser_{Guid.NewGuid():N}";
        var registerDto = new RegisterDto
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = "Test123456",
            FullName = $"Test User {role}",
            Role = role
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return authResponse!.Token;
    }

    public static async Task<(string Token, int UserId)> GetAuthTokenWithUserIdAsync(HttpClient client, UserRole role = UserRole.Receptionist)
    {
        var username = $"testuser_{Guid.NewGuid():N}";
        var registerDto = new RegisterDto
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = "Test123456",
            FullName = $"Test User {role}",
            Role = role
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return (authResponse!.Token, authResponse.Id);
    }

    public static void AddAuthorizationHeader(this HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string requestUri, string token)
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.AddAuthorizationHeader(token);
        return request;
    }
}
