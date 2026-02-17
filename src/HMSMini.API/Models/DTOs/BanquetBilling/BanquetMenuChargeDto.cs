namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetMenuChargeDto
{
    public string? PackageName { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public string? TaxConfigName { get; set; }
    public string? SACCode { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}
