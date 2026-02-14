namespace HMSMini.API.Models.DTOs.Tax;

/// <summary>
/// DTO for applicable tax slab for a specific amount
/// </summary>
public class ApplicableTaxSlabDto
{
    public decimal Amount { get; set; }
    public string SlabRange { get; set; } = string.Empty;
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public string? Description { get; set; }
}
