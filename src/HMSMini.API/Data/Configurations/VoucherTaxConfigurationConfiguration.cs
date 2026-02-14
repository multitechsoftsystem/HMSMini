using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

public class VoucherTaxConfigurationConfiguration : IEntityTypeConfiguration<VoucherTaxConfiguration>
{
    public void Configure(EntityTypeBuilder<VoucherTaxConfiguration> builder)
    {
        builder.ToTable("VoucherTaxConfigurations");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VoucherType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.CgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(v => v.SgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(v => v.IgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.Description)
            .HasMaxLength(500);

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(100);

        builder.Property(v => v.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(v => v.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(v => v.VoucherType)
            .HasDatabaseName("IX_VoucherTaxConfigurations_VoucherType");

        builder.HasIndex(v => v.IsActive)
            .HasDatabaseName("IX_VoucherTaxConfigurations_IsActive");

        builder.HasIndex(v => v.DisplayOrder)
            .HasDatabaseName("IX_VoucherTaxConfigurations_DisplayOrder");

        // Query filter for soft delete
        builder.HasQueryFilter(v => v.DeletedAt == null);

        // Relationships
        builder.HasMany(v => v.AdditionalCharges)
            .WithOne(a => a.VoucherTaxConfig)
            .HasForeignKey(a => a.VoucherTaxConfigId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
