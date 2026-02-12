using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("Payments")]
public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ReceiptNumber { get; set; } = string.Empty;

    [Required]
    public PaymentSourceType SourceType { get; set; }

    public int? CheckInId { get; set; }

    public int? BanquetBookingId { get; set; }

    public int? CompanyId { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public PaymentType PaymentType { get; set; }

    [Required]
    public PaymentMode PaymentMode { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? ReceivedBy { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }

    public int? VoucherId { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn? CheckIn { get; set; }

    [ForeignKey(nameof(BanquetBookingId))]
    public virtual BanquetBooking? BanquetBooking { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public virtual Company? Company { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
