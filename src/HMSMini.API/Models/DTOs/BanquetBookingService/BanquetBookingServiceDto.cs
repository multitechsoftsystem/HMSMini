namespace HMSMini.API.Models.DTOs.BanquetBookingService;

public class BanquetBookingServiceDto
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public int? BanquetServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public DateTime CreatedAt { get; set; }
}
