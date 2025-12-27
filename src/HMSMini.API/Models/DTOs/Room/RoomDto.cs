using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Room;

/// <summary>
/// Room data transfer object
/// </summary>
public class RoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public RoomStatus RoomStatus { get; set; }
    public DateTime? RoomStatusFromDate { get; set; }
    public DateTime? RoomStatusToDate { get; set; }
}
