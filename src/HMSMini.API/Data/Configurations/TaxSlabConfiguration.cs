using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for TaxSlab
/// </summary>
public class TaxSlabConfiguration : IEntityTypeConfiguration<TaxSlab>
{
    public void Configure(EntityTypeBuilder<TaxSlab> builder)
    {
        // Table name
        builder.ToTable("TaxSlabs");

        // Primary key
        builder.HasKey(ts => ts.Id);

        // Properties
        builder.Property(ts => ts.MinAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(ts => ts.MaxAmount)
            .HasColumnType("decimal(10,2)");

        builder.Property(ts => ts.CgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(ts => ts.SgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(ts => ts.IgstPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(ts => ts.EffectiveFrom)
            .IsRequired();

        builder.Property(ts => ts.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ts => ts.Description)
            .HasMaxLength(500);

        builder.Property(ts => ts.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ts => ts.CreatedBy)
            .HasMaxLength(100);

        builder.Property(ts => ts.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(ts => ts.DeletedBy)
            .HasMaxLength(100);

        // Indexes for performance
        builder.HasIndex(ts => new { ts.MinAmount, ts.MaxAmount })
            .HasDatabaseName("IX_TaxSlabs_AmountRange");

        builder.HasIndex(ts => new { ts.EffectiveFrom, ts.EffectiveTo })
            .HasDatabaseName("IX_TaxSlabs_EffectiveDates");

        builder.HasIndex(ts => ts.IsActive);

        // Query filter for soft delete
        builder.HasQueryFilter(ts => ts.DeletedAt == null);
    }
}
