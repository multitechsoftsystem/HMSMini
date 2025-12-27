using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.RoomType;

/// <summary>
/// DTO for creating a new room type
/// </summary>
public class CreateRoomTypeDto
{
    [Required]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty;

    [StringLength(500)]
    public string? RoomDescription { get; set; }
}
