namespace HMSMini.Web.Models;

public class TaxSlabModel
{
    public int Id { get; set; }
    public decimal MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    public string AmountRangeDisplay => MaxAmount.HasValue
        ? $"₹{MinAmount:N2} - ₹{MaxAmount:N2}"
        : $"₹{MinAmount:N2} and above";

    public string TaxRateDisplay => $"{CgstPercentage + SgstPercentage}% (CGST {CgstPercentage}% + SGST {SgstPercentage}%) or IGST {IgstPercentage}%";
}

public class ApplicableTaxSlabModel
{
    public decimal Amount { get; set; }
    public string SlabRange { get; set; } = string.Empty;
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public string? Description { get; set; }
}

public class TaxCalculationRequestModel
{
    public decimal Amount { get; set; }
    public int TaxType { get; set; } // 0 = CGST+SGST, 1 = IGST
    public DateTime? Date { get; set; }
}
