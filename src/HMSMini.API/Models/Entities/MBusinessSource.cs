using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents a business source/booking channel master table
/// </summary>
[Table("MBusinessSources")]
public class MBusinessSource
{
    /// <summary>
    /// Primary key for business source
    /// </summary>
    [Key]
    public int BusinessSourceId { get; set; }

    /// <summary>
    /// Name of the business source (Walk-In, Online, Corporate, Agent, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the business source
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the business source is active
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
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
