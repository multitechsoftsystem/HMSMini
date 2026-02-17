using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class ExpenseVoucherConfiguration : IEntityTypeConfiguration<ExpenseVoucher>
{
    public void Configure(EntityTypeBuilder<ExpenseVoucher> builder)
    {
        builder.ToTable("ExpenseVouchers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VoucherNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.VoucherNumber)
            .IsUnique()
            .HasDatabaseName("IX_ExpenseVouchers_VoucherNumber");

        builder.Property(e => e.VoucherDate).IsRequired();
        builder.Property(e => e.FinancialYearId).IsRequired();
        builder.Property(e => e.ExpenseHeadId).IsRequired();

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(e => e.PaidTo).HasMaxLength(200);
        builder.Property(e => e.PaymentMode).IsRequired();
        builder.Property(e => e.ReferenceNumber).HasMaxLength(200);
        builder.Property(e => e.Narration).HasMaxLength(1000);

        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);
        builder.Property(e => e.DeletedBy).HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(e => e.FinancialYear)
            .WithMany()
            .HasForeignKey(e => e.FinancialYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ExpenseHead)
            .WithMany()
            .HasForeignKey(e => e.ExpenseHeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.BankAccount)
            .WithMany()
            .HasForeignKey(e => e.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JournalEntry)
            .WithMany()
            .HasForeignKey(e => e.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.VoucherDate)
            .HasDatabaseName("IX_ExpenseVouchers_VoucherDate");

        builder.HasIndex(e => e.FinancialYearId)
            .HasDatabaseName("IX_ExpenseVouchers_FinancialYearId");

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
