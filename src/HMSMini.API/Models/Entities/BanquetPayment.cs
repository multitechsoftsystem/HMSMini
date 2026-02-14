using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("BanquetPayments")]
public class BanquetPayment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BanquetBookingId { get; set; }

    [Required]
    [StringLength(50)]
    public string ReceiptNumber { get; set; } = string.Empty;

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public BanquetPaymentType PaymentType { get; set; }

    [Required]
    public BanquetPaymentMode PaymentMode { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? ReceivedBy { get; set; }

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
}
