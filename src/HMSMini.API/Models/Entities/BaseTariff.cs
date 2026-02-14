using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents base tariff rates for room types by occupancy level
/// </summary>
[Table("BaseTariff")]
public class BaseTariff
{
    /// <summary>
    /// Primary key for base tariff
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to room type
    /// </summary>
    [Required]
    public int RoomTypeId { get; set; }

    /// <summary>
    /// Number of occupants (1-6)
    /// </summary>
    [Required]
    [Range(1, 6)]
    public int OccupancyCount { get; set; }

    /// <summary>
    /// Rate per night
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal RatePerNight { get; set; }

    /// <summary>
    /// Effective from date
    /// </summary>
    [Required]
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// Effective to date (null for ongoing)
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Whether the tariff is active
    /// </summary>
    public bool IsActive { get; set; } = true;

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

    // Navigation properties
    [ForeignKey(nameof(RoomTypeId))]
    public virtual MRoomType RoomType { get; set; } = null!;
}
