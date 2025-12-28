namespace HMSMini.Web.Models;

public class RoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int RoomStatus { get; set; }
    public string RoomStatusName { get; set; } = string.Empty;
    public DateTime? RoomStatusFromDate { get; set; }
    public DateTime? RoomStatusToDate { get; set; }
}

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
}

public class RoomTypeDto
{
    public int RoomTypeId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string? RoomDescription { get; set; }
}

public enum RoomStatus
{
    Available = 0,
    Occupied = 1,
    Dirty = 2,
    Maintenance = 3,
    Blocked = 4
}
