namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetInvoiceListDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int BanquetBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
}
