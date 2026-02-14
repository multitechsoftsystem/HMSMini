namespace HMSMini.API.Models.DTOs.Room;

/// <summary>
/// Data transfer object for room availability information
/// </summary>
public class RoomAvailabilityDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public List<DailyAvailability> DailyAvailability { get; set; } = new();
}

/// <summary>
/// Represents availability status for a specific date
/// </summary>
public class DailyAvailability
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = string.Empty; // "Available", "Occupied", "Reserved", "Blocked"
    public string? GuestName { get; set; }
    public string? ReservationNumber { get; set; }
}
