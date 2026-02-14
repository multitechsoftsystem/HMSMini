using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("MBanquetHalls")]
public class MBanquetHall
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string HallName { get; set; } = string.Empty;

    [Required]
    public int MaxCapacity { get; set; }

    public int MinCapacity { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal RentPerEvent { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [StringLength(1000)]
    public string? Features { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    public virtual ICollection<BanquetBooking> Bookings { get; set; } = new List<BanquetBooking>();
}
