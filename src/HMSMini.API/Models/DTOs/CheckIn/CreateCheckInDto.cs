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

    [Required]
    [MinLength(1)]
    [MaxLength(3)]
    public List<CreateGuestDto> Guests { get; set; } = new();
}
