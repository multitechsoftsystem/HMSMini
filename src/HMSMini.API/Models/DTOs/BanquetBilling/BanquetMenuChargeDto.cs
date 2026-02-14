namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetMenuChargeDto
{
    public string? PackageName { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
    public decimal TotalAmount { get; set; }
}
