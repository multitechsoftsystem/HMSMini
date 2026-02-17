using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("ReceiptAllocations")]
public class ReceiptAllocation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ReceiptId { get; set; }

    public int? InvoiceId { get; set; }

    public int? BanquetInvoiceId { get; set; }

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal AllocatedAmount { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ReceiptId))]
    public virtual Receipt Receipt { get; set; } = null!;

    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey(nameof(BanquetInvoiceId))]
    public virtual BanquetInvoice? BanquetInvoice { get; set; }
}
