namespace HMSMini.Web.Models;

public class RoomAvailabilityDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public List<DailyAvailability> DailyAvailability { get; set; } = new();
}

public class DailyAvailability
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? GuestName { get; set; }
    public string? ReservationNumber { get; set; }
}
