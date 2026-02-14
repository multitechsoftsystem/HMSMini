using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetChargeConfiguration : IEntityTypeConfiguration<BanquetCharge>
{
    public void Configure(EntityTypeBuilder<BanquetCharge> builder)
    {
        builder.ToTable("BanquetCharges");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.BanquetBookingId)
            .IsRequired();

        builder.Property(c => c.ChargeDate)
            .IsRequired();

        builder.Property(c => c.ChargeType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(c => c.TotalAmount)
            .HasComputedColumnSql("[Amount] * [Quantity]", stored: true)
            .HasColumnType("decimal(10,2)");

        builder.Property(c => c.ApplyTax)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(c => c.BanquetBooking)
            .WithMany(b => b.Charges)
            .HasForeignKey(c => c.BanquetBookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.VoucherTaxConfig)
            .WithMany()
            .HasForeignKey(c => c.VoucherTaxConfigId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.BanquetBookingId)
            .HasDatabaseName("IX_BanquetCharges_BanquetBookingId");

        builder.HasIndex(c => c.ChargeDate)
            .HasDatabaseName("IX_BanquetCharges_ChargeDate");

        // Query filter for soft delete
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
