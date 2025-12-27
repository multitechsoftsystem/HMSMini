using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents individual room inventory
/// </summary>
[Table("RoomNo")]
public class RoomNo
{
    /// <summary>
    /// Primary key for room
    /// </summary>
    [Key]
    public int RoomId { get; set; }

    /// <summary>
    /// Room number (e.g., 101, 202, etc.)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to room type
    /// </summary>
    [Required]
    public int RoomTypeId { get; set; }

    /// <summary>
    /// Current status of the room
    /// </summary>
    [Required]
    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;

    /// <summary>
    /// Start date when the room status becomes effective
    /// </summary>
    public DateTime? RoomStatusFromDate { get; set; }

    /// <summary>
    /// End date when the room status expires
    /// </summary>
    public DateTime? RoomStatusToDate { get; set; }

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

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
