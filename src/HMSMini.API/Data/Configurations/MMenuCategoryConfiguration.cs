using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MMenuCategoryConfiguration : IEntityTypeConfiguration<MMenuCategory>
{
    public void Configure(EntityTypeBuilder<MMenuCategory> builder)
    {
        builder.ToTable("MMenuCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
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

        // Indexes
        builder.HasIndex(c => c.CategoryName)
            .HasDatabaseName("IX_MMenuCategories_CategoryName");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_MMenuCategories_IsActive");

        builder.HasIndex(c => c.DeletedAt)
            .HasDatabaseName("IX_MMenuCategories_DeletedAt");
    }
}
