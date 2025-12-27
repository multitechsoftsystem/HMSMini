using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents individual guest information (normalized from original schema)
/// </summary>
[Table("Guest")]
public class Guest
{
    /// <summary>
    /// Primary key for guest
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to check-in
    /// </summary>
    [Required]
    public int CheckInId { get; set; }

    /// <summary>
    /// Guest number within the check-in (1, 2, or 3)
    /// </summary>
    [Required]
    [Range(1, 3)]
    public int GuestNumber { get; set; }

    /// <summary>
    /// Full name of the guest
    /// </summary>
    [Required]
    [StringLength(200)]
    public string GuestName { get; set; } = string.Empty;

    /// <summary>
    /// Address of the guest
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [StringLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [StringLength(100)]
    public string? State { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Mobile number
    /// </summary>
    [StringLength(20)]
    public string? MobileNo { get; set; }

    /// <summary>
    /// File path for first ID proof photo
    /// </summary>
    [StringLength(500)]
    public string? Photo1Path { get; set; }

    /// <summary>
    /// File path for second ID proof photo
    /// </summary>
    [StringLength(500)]
    public string? Photo2Path { get; set; }

    // Navigation property
    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn CheckIn { get; set; } = null!;
}
