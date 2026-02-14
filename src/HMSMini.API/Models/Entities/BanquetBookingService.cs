using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("BanquetBookingServices")]
public class BanquetBookingService
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BanquetBookingId { get; set; }

    public int? BanquetServiceId { get; set; }

    [Required]
    [StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Rate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }

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
    [ForeignKey(nameof(BanquetBookingId))]
    public virtual BanquetBooking BanquetBooking { get; set; } = null!;

    [ForeignKey(nameof(BanquetServiceId))]
    public virtual MBanquetService? BanquetService { get; set; }

    [ForeignKey(nameof(VoucherTaxConfigId))]
    public virtual VoucherTaxConfiguration? VoucherTaxConfig { get; set; }
}
