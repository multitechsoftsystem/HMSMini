using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("JournalEntries")]
public class JournalEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string EntryNumber { get; set; } = string.Empty;

    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
    public int FinancialYearId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public JournalSourceType SourceType { get; set; }

    public int? SourceId { get; set; }

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalAmount { get; set; }

    public bool IsReversed { get; set; }

    public int? ReversalOfId { get; set; }

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
    [ForeignKey(nameof(FinancialYearId))]
    public virtual FinancialYear FinancialYear { get; set; } = null!;

    [ForeignKey(nameof(ReversalOfId))]
    public virtual JournalEntry? ReversalOf { get; set; }

    public virtual ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}
