namespace HMSMini.API.Models.DTOs.Tax;

/// <summary>
/// DTO for tax slab information
/// </summary>
public class TaxSlabDto
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
}
