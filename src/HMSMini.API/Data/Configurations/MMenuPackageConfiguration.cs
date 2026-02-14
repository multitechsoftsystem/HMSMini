using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MMenuPackageConfiguration : IEntityTypeConfiguration<MMenuPackage>
{
    public void Configure(EntityTypeBuilder<MMenuPackage> builder)
    {
        builder.ToTable("MMenuPackages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PackageName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.RatePerPlate)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

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
        builder.HasIndex(p => p.PackageName)
            .HasDatabaseName("IX_MMenuPackages_PackageName");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_MMenuPackages_IsActive");

        builder.HasIndex(p => p.DeletedAt)
            .HasDatabaseName("IX_MMenuPackages_DeletedAt");
    }
}
