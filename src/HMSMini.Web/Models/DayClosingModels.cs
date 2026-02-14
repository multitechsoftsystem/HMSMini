namespace HMSMini.Web.Models;

public class WorkingDateModel
{
    public DateTime WorkingDate { get; set; }
    public DateTime SystemDate { get; set; }
    public bool IsDateClosed { get; set; }
    public int DaysBehindSystemDate { get; set; }
}

public class DayCloseValidationModel
{
    public bool CanClose { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public DateTime CurrentWorkingDate { get; set; }
    public int ActiveCheckInsCount { get; set; }
    public int UnpostedAdditionalChargesCount { get; set; }
}

public class DayClosePreviewModel
{
    public DateTime CurrentWorkingDate { get; set; }
    public DateTime NextWorkingDate { get; set; }
    public int ActiveCheckInsCount { get; set; }
    public int TotalVouchersToPost { get; set; }
    public decimal TotalRevenueToPost { get; set; }
    public List<VoucherSummaryModel> VoucherSummary { get; set; } = new();
    public List<CheckInCloseSummaryModel> CheckInSummaries { get; set; } = new();
}

public class CheckInCloseSummaryModel
{
    public int CheckInId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? GuestType { get; set; }
    public bool IsComplimentary { get; set; }
    public int VouchersToPost { get; set; }
    public decimal AmountToPost { get; set; }
}

public class DayCloseResultModel
{
    public bool Success { get; set; }
    public DateTime ClosedDate { get; set; }
    public DateTime NewWorkingDate { get; set; }
    public int TotalActiveCheckIns { get; set; }
    public int TotalVouchersPosted { get; set; }
    public decimal TotalRevenuePosted { get; set; }
    public int DurationSeconds { get; set; }
    public List<VoucherSummaryModel> VoucherSummary { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class DayClosingAuditModel
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
