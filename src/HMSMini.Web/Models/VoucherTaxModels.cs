namespace HMSMini.Web.Models;

public class VoucherTaxConfigModel
{
    public int Id { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string? SACCode { get; set; }
    public int DisplayOrder { get; set; }

    public string TaxRateDisplay => $"{CgstPercentage + SgstPercentage}% (CGST {CgstPercentage}% + SGST {SgstPercentage}%) or IGST {IgstPercentage}%";
}

public class CreateVoucherTaxConfigModel
{
    public string VoucherType { get; set; } = string.Empty;
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? SACCode { get; set; }
    public int DisplayOrder { get; set; }
}
