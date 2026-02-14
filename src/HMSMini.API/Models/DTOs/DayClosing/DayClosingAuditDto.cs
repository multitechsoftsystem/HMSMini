namespace HMSMini.API.Models.DTOs.DayClosing;

/// <summary>
/// Day closing audit record
/// </summary>
public class DayClosingAuditDto
{
    public int Id { get; set; }
    public DateTime ClosedDate { get; set; }
    public DateTime NextWorkingDate { get; set; }
    public int TotalActiveCheckIns { get; set; }
    public int TotalVouchersPosted { get; set; }
    public decimal TotalRevenuePosted { get; set; }
    public string ClosingStatus { get; set; } = string.Empty;
    public string? ErrorLog { get; set; }
    public DateTime ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public int? DurationSeconds { get; set; }
}
