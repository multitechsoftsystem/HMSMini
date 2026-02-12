using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VoucherNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.VoucherNumber)
            .IsUnique()
            .HasDatabaseName("IX_Vouchers_VoucherNumber");

        builder.Property(v => v.VoucherDate)
            .IsRequired();

        builder.HasIndex(v => v.VoucherDate)
            .HasDatabaseName("IX_Vouchers_VoucherDate");

        builder.Property(v => v.PostingDate)
            .IsRequired();

        builder.Property(v => v.VoucherType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.VoucherType)
            .HasDatabaseName("IX_Vouchers_VoucherType");

        builder.Property(v => v.Description)
            .HasMaxLength(500);

        builder.Property(v => v.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.HasIndex(v => v.CheckInId)
            .HasDatabaseName("IX_Vouchers_CheckInId");

        builder.HasIndex(v => v.BanquetBookingId)
            .HasDatabaseName("IX_Vouchers_BanquetBookingId");

        builder.HasIndex(v => v.PaymentId)
            .HasDatabaseName("IX_Vouchers_PaymentId");

        builder.Property(v => v.RoomNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(v => v.PostingStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Posted");

        builder.Property(v => v.AutoPostDaily)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.TaxType)
            .HasMaxLength(20);

        builder.Property(v => v.TaxPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(v => v.TaxableAmount)
            .HasColumnType("decimal(10,2)");

        builder.Property(v => v.PostedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.PostedBy)
            .HasMaxLength(100);

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(100);

        builder.Property(v => v.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(v => v.CancelledBy)
            .HasMaxLength(100);

        builder.Property(v => v.CancellationReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(v => v.CheckIn)
            .WithMany()
            .HasForeignKey(v => v.CheckInId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.BanquetBooking)
            .WithMany()
            .HasForeignKey(v => v.BanquetBookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Payment)
            .WithMany()
            .HasForeignKey(v => v.PaymentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Guest)
            .WithMany()
            .HasForeignKey(v => v.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.AdditionalCharge)
            .WithMany()
            .HasForeignKey(v => v.AdditionalChargeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
