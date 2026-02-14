using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetPayment;

public class BanquetPaymentDto
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public BanquetPaymentType PaymentType { get; set; }
    public BanquetPaymentMode PaymentMode { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
