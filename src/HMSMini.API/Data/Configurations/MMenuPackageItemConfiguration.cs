using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MMenuPackageItemConfiguration : IEntityTypeConfiguration<MMenuPackageItem>
{
    public void Configure(EntityTypeBuilder<MMenuPackageItem> builder)
    {
        builder.ToTable("MMenuPackageItems");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.MenuPackageId)
            .IsRequired();

        builder.Property(pi => pi.MenuItemId)
            .IsRequired();

        builder.Property(pi => pi.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(pi => pi.CreatedBy)
            .HasMaxLength(100);

        builder.Property(pi => pi.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(pi => pi.MenuPackage)
            .WithMany(p => p.PackageItems)
            .HasForeignKey(pi => pi.MenuPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.MenuItem)
            .WithMany(i => i.PackageItems)
            .HasForeignKey(pi => pi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique index on package-item combination
        builder.HasIndex(pi => new { pi.MenuPackageId, pi.MenuItemId })
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL")
            .HasDatabaseName("IX_MMenuPackageItems_Package_Item_Unique");

        // Indexes
        builder.HasIndex(pi => pi.MenuPackageId)
            .HasDatabaseName("IX_MMenuPackageItems_MenuPackageId");

        builder.HasIndex(pi => pi.MenuItemId)
            .HasDatabaseName("IX_MMenuPackageItems_MenuItemId");
    }
}
