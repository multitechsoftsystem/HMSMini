namespace HMSMini.Web.Models;

public class VoucherModel
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public DateTime PostingDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int CheckInId { get; set; }
    public int? GuestId { get; set; }
    public string? GuestName { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string PostingStatus { get; set; } = string.Empty;
    public bool AutoPostDaily { get; set; }
    public string? TaxType { get; set; }
    public decimal? TaxPercentage { get; set; }
    public decimal? TaxableAmount { get; set; }
    public int? AdditionalChargeId { get; set; }
    public DateTime PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
}

public class CreateVoucherModel
{
    public string VoucherType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CheckInId { get; set; }
    public int? GuestId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public bool AutoPostDaily { get; set; } = false;
    public string? TaxType { get; set; }
    public decimal? TaxPercentage { get; set; }
    public decimal? TaxableAmount { get; set; }
    public int? AdditionalChargeId { get; set; }
}

public class CancelVoucherModel
{
    public string CancellationReason { get; set; } = string.Empty;
}

public class VoucherSummaryModel
{
    public string VoucherType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}
