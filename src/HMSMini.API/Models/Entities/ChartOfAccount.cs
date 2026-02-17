using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("ChartOfAccounts")]
public class ChartOfAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    public AccountType AccountType { get; set; }

    public int? ParentAccountId { get; set; }

    public bool IsSystemAccount { get; set; }

    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ParentAccountId))]
    public virtual ChartOfAccount? ParentAccount { get; set; }

    public virtual ICollection<ChartOfAccount> ChildAccounts { get; set; } = new List<ChartOfAccount>();
}
