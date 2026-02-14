namespace HMSMini.API.Models.DTOs.VoucherTax;

public class VoucherTaxConfigDto
{
    public int Id { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    public string TaxRateDisplay => $"{CgstPercentage + SgstPercentage}% (CGST {CgstPercentage}% + SGST {SgstPercentage}%) or IGST {IgstPercentage}%";
}
