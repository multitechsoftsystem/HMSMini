using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Auth;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Implementations;

namespace HMSMini.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Setup configuration for JWT
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "YourSuperSecretKeyThatIsAtLeast32CharactersLongForTesting!"},
            {"JwtSettings:Issuer", "HMSMiniAPI"},
            {"JwtSettings:Audience", "HMSMiniClient"},
            {"JwtSettings:ExpirationHours", "24"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _authService = new AuthService(_context, _configuration, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add a test user
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            FullName = "Test User",
            Role = UserRole.Receptionist,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123",
            FullName = "New User",
            Role = UserRole.Receptionist
        };

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
        result.Email.Should().Be("newuser@example.com");
        result.FullName.Should().Be("New User");
        result.Role.Should().Be(UserRole.Receptionist);
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
        userInDb.Should().NotBeNull();
        userInDb!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "testuser", // Already exists
            Email = "different@example.com",
            Password = "Password123",
            FullName = "Different User",
            Role = UserRole.Receptionist
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*testuser*already taken*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "differentuser",
            Email = "test@example.com", // Already exists
            Password = "Password123",
            FullName = "Different User",
            Role = UserRole.Receptionist
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*test@example.com*already registered*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "Password123"
        };

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        // Verify LastLoginAt was updated
        var userInDb = await _context.Users.FindAsync(1);
        userInDb!.LastLoginAt.Should().NotBeNull();
        userInDb.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new LoginDto
        {
            Username = "nonexistent",
            Password = "Password123"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var inactiveUser = new User
        {
            Id = 2,
            Username = "inactiveuser",
            Email = "inactive@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            FullName = "Inactive User",
            Role = UserRole.Receptionist,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(inactiveUser);
        await _context.SaveChangesAsync();

        var dto = new LoginDto
        {
            Username = "inactiveuser",
            Password = "Password123"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*deactivated*");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Act
        var result = await _authService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Role.Should().Be(UserRole.Receptionist);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _authService.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllActiveUsers()
    {
        // Arrange
        var user2 = new User
        {
            Id = 2,
            Username = "user2",
            Email = "user2@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            FullName = "User Two",
            Role = UserRole.Manager,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Username == "testuser");
        result.Should().Contain(u => u.Username == "user2");
    }

    [Fact]
    public async Task DeactivateUserAsync_WithValidId_ShouldDeactivateUser()
    {
        // Act
        await _authService.DeactivateUserAsync(1);

        // Assert
        var userInDb = await _context.Users.FindAsync(1);
        userInDb.Should().NotBeNull();
        userInDb!.IsActive.Should().BeFalse();
        userInDb.UpdatedAt.Should().NotBeNull();
        userInDb.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeactivateUserAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _authService.DeactivateUserAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*User*999*");
    }

    [Fact]
    public async Task RegisterAsync_ShouldHashPassword()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "hashtest",
            Email = "hashtest@example.com",
            Password = "PlainTextPassword123",
            FullName = "Hash Test",
            Role = UserRole.Receptionist
        };

        // Act
        await _authService.RegisterAsync(dto);

        // Assert
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "hashtest");
        userInDb.Should().NotBeNull();
        userInDb!.PasswordHash.Should().NotBe("PlainTextPassword123");
        userInDb.PasswordHash.Should().StartWith("$2");  // BCrypt hash format

        // Verify password can be validated
        BCrypt.Net.BCrypt.Verify("PlainTextPassword123", userInDb.PasswordHash).Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
