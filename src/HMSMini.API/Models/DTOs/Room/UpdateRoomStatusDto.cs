using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Room;

/// <summary>
/// DTO for updating room status
/// </summary>
public class UpdateRoomStatusDto
{
    [Required]
    public RoomStatus RoomStatus { get; set; }

    public DateTime? RoomStatusFromDate { get; set; }

    public DateTime? RoomStatusToDate { get; set; }
}
