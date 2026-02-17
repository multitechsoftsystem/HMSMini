namespace HMSMini.Web.Models;

// Enums
public enum PaymentSourceType
{
    Room = 0,
    Banquet = 1
}

public enum PaymentType
{
    Advance = 0,
    PartialPayment = 1,
    FinalSettlement = 2,
    Refund = 3
}

public enum PaymentMode
{
    Cash = 0,
    Card = 1,
    UPI = 2,
    BankTransfer = 3,
    Cheque = 4
}

// Payment Models
public class PaymentModel
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public PaymentSourceType SourceType { get; set; }
    public int? CheckInId { get; set; }
    public int? BanquetBookingId { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Remarks { get; set; }
    public int? VoucherId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreatePaymentModel
{
    public PaymentSourceType SourceType { get; set; }
    public int? CheckInId { get; set; }
    public int? BanquetBookingId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Today;
    public PaymentType PaymentType { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Remarks { get; set; }
}

public class PaymentSummaryModel
{
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public int PaymentCount { get; set; }
    public List<PaymentModel> Payments { get; set; } = new();
}
