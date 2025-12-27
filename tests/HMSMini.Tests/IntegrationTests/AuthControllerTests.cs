using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.Auth;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturn201AndToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Test123456",
            FullName = "Test User",
            Role = UserRole.Receptionist
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ShouldReturn400()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "duplicate",
            Email = "user1@example.com",
            Password = "Test123456",
            FullName = "User One",
            Role = UserRole.Receptionist
        };

        // Register first user
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Try to register with same username
        registerDto.Email = "user2@example.com";

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturn400()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser2",
            Email = "invalid-email",
            Password = "Test123456",
            FullName = "Test User",
            Role = UserRole.Receptionist
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200AndToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "Test123456",
            FullName = "Login User",
            Role = UserRole.Receptionist
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Username = "loginuser",
            Password = "Test123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result!.Username.Should().Be("loginuser");
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ShouldReturn400()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "nonexistent",
            Password = "Test123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturn400()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "wrongpassuser",
            Email = "wrongpass@example.com",
            Password = "CorrectPassword123",
            FullName = "Wrong Pass User",
            Role = UserRole.Receptionist
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Username = "wrongpassuser",
            Password = "WrongPassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ShouldReturn200AndUserInfo()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "meuser",
            Email = "me@example.com",
            Password = "Test123456",
            FullName = "Me User",
            Role = UserRole.Receptionist
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.Username.Should().Be("meuser");
        result.Email.Should().Be("me@example.com");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ShouldReturn401()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_AsAdmin_ShouldReturn200()
    {
        // Arrange
        var adminRegisterDto = new RegisterDto
        {
            Username = "admin",
            Email = "admin@example.com",
            Password = "Admin123456",
            FullName = "Admin User",
            Role = UserRole.Admin
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", adminRegisterDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(u => u.Username == "admin");
    }

    [Fact]
    public async Task GetAllUsers_AsReceptionist_ShouldReturn403()
    {
        // Arrange
        var receptionistRegisterDto = new RegisterDto
        {
            Username = "receptionist",
            Email = "receptionist@example.com",
            Password = "Recep123456",
            FullName = "Receptionist User",
            Role = UserRole.Receptionist
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", receptionistRegisterDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeactivateUser_AsAdmin_ShouldReturn204()
    {
        // Arrange
        // Create admin
        var adminRegisterDto = new RegisterDto
        {
            Username = "admin2",
            Email = "admin2@example.com",
            Password = "Admin123456",
            FullName = "Admin User",
            Role = UserRole.Admin
        };
        var adminRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", adminRegisterDto);
        var adminAuthResponse = await adminRegisterResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Create user to deactivate
        var userRegisterDto = new RegisterDto
        {
            Username = "todeactivate",
            Email = "deactivate@example.com",
            Password = "Test123456",
            FullName = "To Deactivate",
            Role = UserRole.Receptionist
        };
        var userRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", userRegisterDto);
        var userAuthResponse = await userRegisterResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/{userAuthResponse!.Id}/deactivate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminAuthResponse!.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeactivateUser_AsReceptionist_ShouldReturn403()
    {
        // Arrange
        var receptionistRegisterDto = new RegisterDto
        {
            Username = "receptionist2",
            Email = "receptionist2@example.com",
            Password = "Recep123456",
            FullName = "Receptionist User",
            Role = UserRole.Receptionist
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", receptionistRegisterDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/auth/{authResponse!.Id}/deactivate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
