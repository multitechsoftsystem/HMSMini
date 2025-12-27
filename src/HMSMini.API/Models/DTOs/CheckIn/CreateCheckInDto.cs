using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.DTOs.Guest;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// DTO for creating a new check-in
/// </summary>
public class CreateCheckInDto
{
    [Required]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    public DateTime? ActualCheckInDate { get; set; }

    [StringLength(50)]
    public string? RegistrationNo { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(3)]
    public List<CreateGuestDto> Guests { get; set; } = new();
}
