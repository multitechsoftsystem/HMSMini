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
    [ForeignKey(nameof(RoomId))]
    public virtual RoomNo Room { get; set; } = null!;

    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();
}
