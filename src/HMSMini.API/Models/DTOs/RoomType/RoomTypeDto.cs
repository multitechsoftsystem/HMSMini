namespace HMSMini.API.Models.DTOs.RoomType;

/// <summary>
/// Room type data transfer object
/// </summary>
public class RoomTypeDto
{
    public int RoomTypeId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string? RoomDescription { get; set; }
}
