namespace HMSMini.API.Models.DTOs.DayClosing;

/// <summary>
/// Working date information
/// </summary>
public class WorkingDateDto
{
    public DateTime WorkingDate { get; set; }
    public DateTime SystemDate { get; set; }
    public bool IsDateClosed { get; set; }
    public int DaysBehindSystemDate { get; set; }
}
