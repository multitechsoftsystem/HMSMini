using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Master table for room types (Single, Double, Suite, etc.)
/// </summary>
[Table("MRoomTypes")]
public class MRoomType
{
    /// <summary>
    /// Primary key for room type
    /// </summary>
    [Key]
    public int RoomTypeId { get; set; }

    /// <summary>
    /// Name of the room type (e.g., Single, Double, Suite)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the room type and amenities
    /// </summary>
    [StringLength(500)]
    public string? RoomDescription { get; set; }

    // Navigation property
    public virtual ICollection<RoomNo> Rooms { get; set; } = new List<RoomNo>();
}
