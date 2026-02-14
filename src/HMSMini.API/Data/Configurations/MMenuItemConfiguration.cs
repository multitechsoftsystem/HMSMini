using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MMenuItemConfiguration : IEntityTypeConfiguration<MMenuItem>
{
    public void Configure(EntityTypeBuilder<MMenuItem> builder)
    {
        builder.ToTable("MMenuItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.MenuCategoryId)
            .IsRequired();

        builder.Property(i => i.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.ItemType)
            .IsRequired();

        builder.Property(i => i.PricePerPlate)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(i => i.MenuCategory)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.MenuCategoryId)
            .HasDatabaseName("IX_MMenuItems_MenuCategoryId");

        builder.HasIndex(i => i.ItemName)
            .HasDatabaseName("IX_MMenuItems_ItemName");

        builder.HasIndex(i => i.IsActive)
            .HasDatabaseName("IX_MMenuItems_IsActive");

        builder.HasIndex(i => i.DeletedAt)
            .HasDatabaseName("IX_MMenuItems_DeletedAt");
    }
}
