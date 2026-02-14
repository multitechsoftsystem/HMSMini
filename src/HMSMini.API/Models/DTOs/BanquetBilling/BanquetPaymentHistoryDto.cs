namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetPaymentHistoryDto
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}
