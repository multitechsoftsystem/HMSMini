using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Room;

/// <summary>
/// DTO for creating a new room
/// </summary>
public class CreateRoomDto
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public int RoomTypeId { get; set; }

    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;

    public DateTime? RoomStatusFromDate { get; set; }

    public DateTime? RoomStatusToDate { get; set; }
}
