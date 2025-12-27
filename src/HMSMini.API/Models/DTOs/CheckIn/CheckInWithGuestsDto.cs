using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// CheckIn with guests information
/// </summary>
public class CheckInWithGuestsDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public int Pax { get; set; }
    public CheckInStatus Status { get; set; }
    public List<GuestDto> Guests { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
