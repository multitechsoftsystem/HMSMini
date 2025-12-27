using HMSMini.API.Models.DTOs.Auth;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for authentication and authorization
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int id);

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    Task<List<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    Task DeactivateUserAsync(int id);
}
