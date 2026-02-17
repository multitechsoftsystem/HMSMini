namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetServiceChargeDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
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
