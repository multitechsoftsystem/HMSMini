using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class AdditionalChargeConfiguration : IEntityTypeConfiguration<AdditionalCharge>
{
    public void Configure(EntityTypeBuilder<AdditionalCharge> builder)
    {
        builder.ToTable("AdditionalCharges");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CheckInId)
            .IsRequired();

        builder.Property(a => a.ChargeDate)
            .IsRequired();

        builder.Property(a => a.ChargeType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(a => a.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        // Computed column for TotalAmount
        builder.Property(a => a.TotalAmount)
            .HasComputedColumnSql("[Amount] * [Quantity]", stored: true)
            .HasColumnType("decimal(10,2)");

        builder.Property(a => a.AddedBy)
            .HasMaxLength(100);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.DeletedBy)
            .HasMaxLength(100);

        // Voucher posting tracking
        builder.Property(a => a.IsPostedToVoucher)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign keys
        builder.HasOne(a => a.CheckIn)
            .WithMany()
            .HasForeignKey(a => a.CheckInId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PostedVoucher)
            .WithMany()
            .HasForeignKey(a => a.PostedVoucherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.CheckInId)
            .HasDatabaseName("IX_AdditionalCharges_CheckInId");

        builder.HasIndex(a => a.ChargeDate)
            .HasDatabaseName("IX_AdditionalCharges_ChargeDate");

        // Query filter for soft delete
        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
