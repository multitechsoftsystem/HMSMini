using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents a guest check-in record
/// </summary>
[Table("CheckIn")]
public class CheckIn
{
    /// <summary>
    /// Primary key for check-in
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to room
    /// </summary>
    [Required]
    public int RoomId { get; set; }

    /// <summary>
    /// Check-in date and time
    /// </summary>
    [Required]
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// Expected check-out date and time
    /// </summary>
    [Required]
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// Actual check-in date and time (when guest actually checked in)
    /// </summary>
    public DateTime? ActualCheckInDate { get; set; }

    /// <summary>
    /// Actual check-out date (null until guest checks out)
    /// </summary>
    public DateTime? ActualCheckOutDate { get; set; }

    /// <summary>
    /// Registration number for this check-in
    /// </summary>
    [StringLength(50)]
    public string? RegistrationNo { get; set; }

    /// <summary>
    /// Number of guests (Pax)
    /// </summary>
    [Required]
    [Range(1, 10)]
    public int Pax { get; set; }

    /// <summary>
    /// Current status of the check-in
    /// </summary>
    [Required]
    public CheckInStatus Status { get; set; } = CheckInStatus.Active;

    /// <summary>
    /// Additional remarks or notes
    /// </summary>
    [StringLength(1000)]
    public string? Remarks { get; set; }

    /// <summary>
    /// Foreign key to company (null for walk-in guests)
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Foreign key to business source (booking channel)
    /// </summary>
    public int? BusinessSourceId { get; set; }

    /// <summary>
    /// Foreign key to meal plan
    /// </summary>
    public int? MealPlanId { get; set; }

    /// <summary>
    /// Foreign key to guest type (Normal, Complimentary, Family, etc.)
    /// </summary>
    public int? GuestTypeId { get; set; }

    /// <summary>
    /// Meal plan rate applied at check-in time (total meal plan rate per night)
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? MealPlanRate { get; set; }

    /// <summary>
    /// Tariff rate applied at check-in time
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? TariffApplied { get; set; }

    /// <summary>
    /// Discount percentage applied (0-100)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; } = 0;

    /// <summary>
    /// Final amount after discount
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? FinalAmount { get; set; }

    /// <summary>
    /// Tax type for this check-in (IGST or CGST+SGST)
    /// </summary>
    [Required]
    public TaxType TaxType { get; set; } = TaxType.CgstSgst;

    /// <summary>
    /// Tax slab snapshot at check-in time (stored as JSON for historical accuracy)
    /// </summary>
    public string? TaxSlabSnapshotJson { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who created the record
    /// </summary>
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User who deleted the record
    /// </summary>
    [StringLength(100)]
    public string? DeletedBy { get; set; }

    [ForeignKey(nameof(RoomId))]
    public virtual RoomNo Room { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    public virtual Company? Company { get; set; }

    [ForeignKey(nameof(BusinessSourceId))]
    public virtual MBusinessSource? BusinessSource { get; set; }

    [ForeignKey(nameof(MealPlanId))]
    public virtual MMealPlan? MealPlan { get; set; }

    [ForeignKey(nameof(GuestTypeId))]
    public virtual MGuestType? GuestType { get; set; }

    /// <summary>
    /// Whether this check-in is part of a shared room (room splitting)
    /// </summary>
    public bool IsSharedRoom { get; set; } = false;

    /// <summary>
    /// Group identifier for shared room check-ins (uses the first check-in's ID)
    /// </summary>
    public int? SharedGroupId { get; set; }

    // Navigation properties
    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
