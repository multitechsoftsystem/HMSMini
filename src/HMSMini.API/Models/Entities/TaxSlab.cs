namespace HMSMini.API.Models.Entities;

public class TaxSlab
{
    public int Id { get; set; }

    public decimal MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }  // null = no upper limit

    public decimal CgstPercentage { get; set; }

    public decimal SgstPercentage { get; set; }

    public decimal IgstPercentage { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }
}
