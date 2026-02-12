namespace HMSMini.API.Models.DTOs.Payment;

public class PaymentSummaryDto
{
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public int PaymentCount { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
}
