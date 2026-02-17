using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class ReceiptAllocationConfiguration : IEntityTypeConfiguration<ReceiptAllocation>
{
    public void Configure(EntityTypeBuilder<ReceiptAllocation> builder)
    {
        builder.ToTable("ReceiptAllocations");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ReceiptId).IsRequired();

        builder.Property(a => a.AllocatedAmount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        // Relationships
        builder.HasOne(a => a.Receipt)
            .WithMany(r => r.Allocations)
            .HasForeignKey(a => a.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Invoice)
            .WithMany()
            .HasForeignKey(a => a.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.BanquetInvoice)
            .WithMany()
            .HasForeignKey(a => a.BanquetInvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.ReceiptId)
            .HasDatabaseName("IX_ReceiptAllocations_ReceiptId");

        builder.HasIndex(a => a.InvoiceId)
            .HasDatabaseName("IX_ReceiptAllocations_InvoiceId");

        builder.HasIndex(a => a.BanquetInvoiceId)
            .HasDatabaseName("IX_ReceiptAllocations_BanquetInvoiceId");
    }
}
