namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetServiceChargeDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
}
