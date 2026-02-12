using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Represents a company/corporate client with negotiated rates
/// </summary>
[Table("Company")]
public class Company
{
    /// <summary>
    /// Primary key for company
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Company address
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
    /// GST Number
    /// </summary>
    [StringLength(50)]
    public string? GSTNumber { get; set; }

    /// <summary>
    /// Contact person name
    /// </summary>
    [StringLength(200)]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Designation of contact person
    /// </summary>
    [StringLength(100)]
    public string? Designation { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [StringLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Contact number
    /// </summary>
    [StringLength(20)]
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Discount percentage applied on tariffs (0-100)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; } = 0;

    /// <summary>
    /// Whether the company is active
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
    public virtual ICollection<CompanyTariff> CompanyTariffs { get; set; } = new List<CompanyTariff>();
    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
