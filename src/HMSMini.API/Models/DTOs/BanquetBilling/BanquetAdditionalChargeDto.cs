namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetAdditionalChargeDto
{
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
}
