using HMSMini.API.Models.DTOs.Voucher;

namespace HMSMini.API.Models.DTOs.DayClosing;

/// <summary>
/// Result of day close operation
/// </summary>
public class DayCloseResultDto
{
    public bool Success { get; set; }
    public DateTime ClosedDate { get; set; }
    public DateTime NewWorkingDate { get; set; }
    public int TotalActiveCheckIns { get; set; }
    public int TotalVouchersPosted { get; set; }
    public decimal TotalRevenuePosted { get; set; }

    /// <summary>
    /// Number of check-ins that had checkout dates auto-extended
    /// </summary>
    public int ExtendedCheckInsCount { get; set; }

    public int DurationSeconds { get; set; }
    public List<VoucherSummaryDto> VoucherSummary { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
