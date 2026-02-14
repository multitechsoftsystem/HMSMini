using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfiguration");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TaxType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.TaxPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.ApplicableOn)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("All");

        builder.Property(t => t.EffectiveFrom)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(100);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(t => t.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(t => new { t.EffectiveFrom, t.EffectiveTo })
            .HasDatabaseName("IX_TaxConfiguration_EffectiveDates");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_TaxConfiguration_IsActive");

        // Query filter for soft delete
        builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}
