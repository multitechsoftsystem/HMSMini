using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// CheckIn data transfer object
/// </summary>
public class CheckInDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public int Pax { get; set; }
    public CheckInStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
