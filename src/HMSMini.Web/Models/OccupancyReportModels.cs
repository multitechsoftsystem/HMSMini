namespace HMSMini.Web.Models;

/// <summary>
/// Occupancy report for a specific date
/// </summary>
public class OccupancyReportDto
{
    public DateTime ReportDate { get; set; }
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int CheckedOutRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int DirtyRooms { get; set; }
    public int MaintenanceRooms { get; set; }
    public int BlockedRooms { get; set; }
    public decimal OccupancyPercentage { get; set; }
    public List<RoomTypeOccupancyDto> RoomTypeBreakdown { get; set; } = new();
}

/// <summary>
/// Occupancy breakdown by room type
/// </summary>
public class RoomTypeOccupancyDto
{
    public string RoomTypeName { get; set; } = string.Empty;
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int CheckedOutRooms { get; set; }
    public int AvailableRooms { get; set; }
    public decimal OccupancyPercentage { get; set; }
}
