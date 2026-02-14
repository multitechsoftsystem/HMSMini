using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Meal plan rates for different room types
/// </summary>
[Table("MealPlanRates")]
public class MealPlanRate
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to meal plan
    /// </summary>
    [Required]
    public int MealPlanId { get; set; }

    /// <summary>
    /// Foreign key to room type
    /// </summary>
    [Required]
    public int RoomTypeId { get; set; }

    /// <summary>
    /// Rate per person per night
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal RatePerPersonPerNight { get; set; }

    /// <summary>
    /// Start date for this rate
    /// </summary>
    [Required]
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// End date for this rate (null means no end date)
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Whether this rate is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // Soft delete
    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(MealPlanId))]
    public virtual MMealPlan MealPlan { get; set; } = null!;

    [ForeignKey(nameof(RoomTypeId))]
    public virtual MRoomType RoomType { get; set; } = null!;
}
