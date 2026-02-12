using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Payment;

public class PaymentDto
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
