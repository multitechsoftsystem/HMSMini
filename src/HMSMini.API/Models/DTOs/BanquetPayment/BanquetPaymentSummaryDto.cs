namespace HMSMini.API.Models.DTOs.BanquetPayment;

public class BanquetPaymentSummaryDto
{
    public int BanquetBookingId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public int PaymentCount { get; set; }
    public List<BanquetPaymentDto> Payments { get; set; } = new();
}
