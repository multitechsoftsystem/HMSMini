using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class PaymentVoucherConfiguration : IEntityTypeConfiguration<PaymentVoucher>
{
    public void Configure(EntityTypeBuilder<PaymentVoucher> builder)
    {
        builder.ToTable("PaymentVouchers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.VoucherNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.VoucherNumber)
            .IsUnique()
            .HasDatabaseName("IX_PaymentVouchers_VoucherNumber");

        builder.Property(p => p.VoucherDate).IsRequired();
        builder.Property(p => p.FinancialYearId).IsRequired();

        builder.Property(p => p.PayeeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(p => p.PaymentMode).IsRequired();
        builder.Property(p => p.ReferenceNumber).HasMaxLength(200);
        builder.Property(p => p.Narration).HasMaxLength(1000);

        builder.Property(p => p.CreatedBy).HasMaxLength(100);
        builder.Property(p => p.UpdatedBy).HasMaxLength(100);
        builder.Property(p => p.DeletedBy).HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.FinancialYear)
            .WithMany()
            .HasForeignKey(p => p.FinancialYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.BankAccount)
            .WithMany()
            .HasForeignKey(p => p.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ExpenseVoucher)
            .WithMany()
            .HasForeignKey(p => p.ExpenseVoucherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.JournalEntry)
            .WithMany()
            .HasForeignKey(p => p.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.VoucherDate)
            .HasDatabaseName("IX_PaymentVouchers_VoucherDate");

        builder.HasIndex(p => p.FinancialYearId)
            .HasDatabaseName("IX_PaymentVouchers_FinancialYearId");

        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
