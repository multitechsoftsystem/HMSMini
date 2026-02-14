using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("MBanquetServices")]
public class MBanquetService
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal DefaultRate { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }

    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(VoucherTaxConfigId))]
    public virtual VoucherTaxConfiguration? VoucherTaxConfig { get; set; }
}
