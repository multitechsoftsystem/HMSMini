using HMSMini.API.Models.DTOs.Voucher;

namespace HMSMini.API.Models.DTOs.DayClosing;

/// <summary>
/// Preview of vouchers to be posted during day close
/// </summary>
public class DayClosePreviewDto
{
    public DateTime CurrentWorkingDate { get; set; }
    public DateTime NextWorkingDate { get; set; }
    public int ActiveCheckInsCount { get; set; }
    public int TotalVouchersToPost { get; set; }
    public decimal TotalRevenueToPost { get; set; }
    public List<VoucherSummaryDto> VoucherSummary { get; set; } = new();
    public List<CheckInCloseSummary> CheckInSummaries { get; set; } = new();
}

/// <summary>
/// Summary of a check-in for day close preview
/// </summary>
public class CheckInCloseSummary
{
    public int CheckInId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? GuestType { get; set; }
    public bool IsComplimentary { get; set; }
    public int VouchersToPost { get; set; }
    public decimal AmountToPost { get; set; }
}
