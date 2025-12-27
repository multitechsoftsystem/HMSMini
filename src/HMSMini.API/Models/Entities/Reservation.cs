using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents a future room reservation
/// </summary>
public class Reservation
{
    public int Id { get; set; }

    /// <summary>
    /// Unique reservation confirmation number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ReservationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to RoomNo
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Navigation property to Room
    /// </summary>
    public RoomNo Room { get; set; } = null!;

    /// <summary>
    /// Expected check-in date
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// Expected check-out date
    /// </summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// Number of guests expected
    /// </summary>
    public int NumberOfGuests { get; set; }

    /// <summary>
    /// Primary guest name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string GuestName { get; set; } = string.Empty;

    /// <summary>
    /// Primary guest email
    /// </summary>
    [MaxLength(255)]
    public string? GuestEmail { get; set; }

    /// <summary>
    /// Primary guest mobile number
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string GuestMobile { get; set; } = string.Empty;

    /// <summary>
    /// Special requests or notes
    /// </summary>
    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }

    /// <summary>
    /// Current status of the reservation
    /// </summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    /// <summary>
    /// Foreign key to CheckIn (null until guest checks in)
    /// </summary>
    public int? CheckInId { get; set; }

    /// <summary>
    /// Navigation property to CheckIn
    /// </summary>
    public CheckIn? CheckIn { get; set; }

    /// <summary>
    /// Date and time when reservation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when reservation was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who created the reservation
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// User who last updated the reservation
    /// </summary>
    public int? UpdatedBy { get; set; }
}
