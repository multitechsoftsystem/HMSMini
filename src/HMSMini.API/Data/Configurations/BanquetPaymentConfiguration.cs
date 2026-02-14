using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetPaymentConfiguration : IEntityTypeConfiguration<BanquetPayment>
{
    public void Configure(EntityTypeBuilder<BanquetPayment> builder)
    {
        builder.ToTable("BanquetPayments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.BanquetBookingId)
            .IsRequired();

        builder.Property(p => p.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_BanquetPayments_ReceiptNumber");

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

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(p => p.BanquetBooking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BanquetBookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.BanquetBookingId)
            .HasDatabaseName("IX_BanquetPayments_BanquetBookingId");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_BanquetPayments_PaymentDate");

        // Query filter for soft delete
        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
