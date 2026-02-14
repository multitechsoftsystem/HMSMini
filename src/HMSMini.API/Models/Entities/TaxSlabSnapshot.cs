namespace HMSMini.API.Models.Entities;

public class TaxSlabSnapshot
{
    public List<TaxSlabSnapshotItem> Slabs { get; set; } = new();

    public DateTime SnapshotDate { get; set; }
}

public class TaxSlabSnapshotItem
{
    public decimal MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public decimal CgstPercentage { get; set; }

    public decimal SgstPercentage { get; set; }

    public decimal IgstPercentage { get; set; }

    public string? Description { get; set; }
}
