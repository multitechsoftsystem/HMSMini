using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_Payments_ReceiptNumber");

        builder.Property(p => p.SourceType)
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .IsRequired();

        builder.Property(p => p.PaymentMode)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(200);

        builder.Property(p => p.ReceivedBy)
            .HasMaxLength(100);

        builder.Property(p => p.Remarks)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.CheckInId)
            .HasDatabaseName("IX_Payments_CheckInId");

        builder.HasIndex(p => p.BanquetBookingId)
            .HasDatabaseName("IX_Payments_BanquetBookingId");

        builder.HasIndex(p => p.CompanyId)
            .HasDatabaseName("IX_Payments_CompanyId");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_Payments_PaymentDate");

        builder.HasIndex(p => p.SourceType)
            .HasDatabaseName("IX_Payments_SourceType");

        // Relationships
        builder.HasOne(p => p.CheckIn)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CheckInId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.BanquetBooking)
            .WithMany(b => b.UnifiedPayments)
            .HasForeignKey(p => p.BanquetBookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Company)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Voucher)
            .WithMany()
            .HasForeignKey(p => p.VoucherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete query filter
        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
