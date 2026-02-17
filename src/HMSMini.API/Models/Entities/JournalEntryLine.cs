using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("JournalEntryLines")]
public class JournalEntryLine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int JournalEntryId { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal DebitAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal CreditAmount { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation properties
    [ForeignKey(nameof(JournalEntryId))]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccount Account { get; set; } = null!;
}
