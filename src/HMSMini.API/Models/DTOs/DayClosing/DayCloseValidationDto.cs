namespace HMSMini.API.Models.DTOs.DayClosing;

/// <summary>
/// Result of day close validation
/// </summary>
public class DayCloseValidationDto
{
    public bool CanClose { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public DateTime CurrentWorkingDate { get; set; }
    public int ActiveCheckInsCount { get; set; }
    public int UnpostedAdditionalChargesCount { get; set; }

    /// <summary>
    /// Number of check-ins that will have checkout dates auto-extended during day close
    /// </summary>
    public int CheckInsToExtendCount { get; set; }
}
