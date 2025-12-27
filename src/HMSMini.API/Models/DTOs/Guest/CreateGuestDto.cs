using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Guest;

/// <summary>
/// DTO for creating a guest
/// </summary>
public class CreateGuestDto
{
    [Required]
    [StringLength(200)]
    public string GuestName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? MobileNo { get; set; }

    [StringLength(20)]
    public string? PanOrAadharNo { get; set; }
}
