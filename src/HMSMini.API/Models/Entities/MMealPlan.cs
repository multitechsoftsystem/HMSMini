using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Master table for meal plan types (EP, CP, MAP, AP)
/// </summary>
[Table("MMealPlans")]
public class MMealPlan
{
    [Key]
    public int MealPlanId { get; set; }

    /// <summary>
    /// Meal plan code (EP, CP, MAP, AP)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string PlanCode { get; set; } = string.Empty;

    /// <summary>
    /// Meal plan name (Room Only, Breakfast, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PlanName { get; set; } = string.Empty;

    /// <summary>
    /// Description of what's included in the meal plan
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Whether this meal plan is currently active
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
    public virtual ICollection<MealPlanRate> MealPlanRates { get; set; } = new List<MealPlanRate>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
