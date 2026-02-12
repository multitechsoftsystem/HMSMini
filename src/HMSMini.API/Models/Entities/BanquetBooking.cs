using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("BanquetBookings")]
public class BanquetBooking
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string BookingNumber { get; set; } = string.Empty;

    [Required]
    public int BanquetHallId { get; set; }

    [Required]
    public int EventTypeId { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public TimeSpan EventStartTime { get; set; }

    [Required]
    public TimeSpan EventEndTime { get; set; }

    [Required]
    public int ExpectedGuests { get; set; }

    public int? ActualGuests { get; set; }

    [Required]
    public BanquetBookingStatus Status { get; set; } = BanquetBookingStatus.Enquiry;

    [Required]
    public BanquetPricingType PricingType { get; set; } = BanquetPricingType.PerPlate;

    [Required]
    [StringLength(200)]
    public string ContactPersonName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    public int? CompanyId { get; set; }

    public int? CheckInId { get; set; }

    [Required]
    public TaxType TaxType { get; set; } = TaxType.CgstSgst;

    [Column(TypeName = "nvarchar(max)")]
    public string? TaxSlabSnapshotJson { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    public decimal HallRent { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }

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
    [ForeignKey(nameof(BanquetHallId))]
    public virtual MBanquetHall BanquetHall { get; set; } = null!;

    [ForeignKey(nameof(EventTypeId))]
    public virtual MEventType EventType { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    public virtual Company? Company { get; set; }

    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn? CheckIn { get; set; }

    public virtual ICollection<BanquetBookingMenu> BookingMenus { get; set; } = new List<BanquetBookingMenu>();
    public virtual ICollection<BanquetBookingService> BookingServices { get; set; } = new List<BanquetBookingService>();
    public virtual ICollection<BanquetCharge> Charges { get; set; } = new List<BanquetCharge>();
    public virtual ICollection<BanquetPayment> Payments { get; set; } = new List<BanquetPayment>();
    public virtual ICollection<Payment> UnifiedPayments { get; set; } = new List<Payment>();
}
