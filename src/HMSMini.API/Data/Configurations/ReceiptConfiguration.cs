using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_Receipts_ReceiptNumber");

        builder.Property(r => r.ReceiptDate).IsRequired();
        builder.Property(r => r.FinancialYearId).IsRequired();

        builder.Property(r => r.ReceivedFrom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Amount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(r => r.PaymentMode).IsRequired();
        builder.Property(r => r.ReferenceNumber).HasMaxLength(200);
        builder.Property(r => r.Narration).HasMaxLength(1000);

        builder.Property(r => r.CreatedBy).HasMaxLength(100);
        builder.Property(r => r.UpdatedBy).HasMaxLength(100);
        builder.Property(r => r.DeletedBy).HasMaxLength(100);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(r => r.FinancialYear)
            .WithMany()
            .HasForeignKey(r => r.FinancialYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.BankAccount)
            .WithMany()
            .HasForeignKey(r => r.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.JournalEntry)
            .WithMany()
            .HasForeignKey(r => r.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.ReceiptDate)
            .HasDatabaseName("IX_Receipts_ReceiptDate");

        builder.HasIndex(r => r.FinancialYearId)
            .HasDatabaseName("IX_Receipts_FinancialYearId");

        builder.HasIndex(r => r.CompanyId)
            .HasDatabaseName("IX_Receipts_CompanyId");

        builder.HasQueryFilter(r => r.DeletedAt == null);
    }
}
