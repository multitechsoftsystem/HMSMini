using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Auth;

/// <summary>
/// DTO for authentication response containing JWT token and user info
/// </summary>
public class AuthResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
