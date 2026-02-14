using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

public class GuestTypeConfiguration : IEntityTypeConfiguration<MGuestType>
{
    public void Configure(EntityTypeBuilder<MGuestType> builder)
    {
        builder.ToTable("MGuestTypes");

        builder.HasKey(gt => gt.GuestTypeId);

        builder.Property(gt => gt.TypeName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(gt => gt.Description)
            .HasMaxLength(200);

        builder.Property(gt => gt.IsActive)
            .HasDefaultValue(true);

        builder.Property(gt => gt.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Seed default guest types
        builder.HasData(
            new MGuestType { GuestTypeId = 1, TypeName = "Normal", Description = "Regular paying guest", IsActive = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new MGuestType { GuestTypeId = 2, TypeName = "Complimentary", Description = "Complimentary stay", IsActive = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new MGuestType { GuestTypeId = 3, TypeName = "Family", Description = "Family member stay", IsActive = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new MGuestType { GuestTypeId = 4, TypeName = "Hotel2", Description = "Hotel2 guest type", IsActive = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new MGuestType { GuestTypeId = 5, TypeName = "Hotel3", Description = "Hotel3 guest type", IsActive = true, DisplayOrder = 5, CreatedAt = DateTime.UtcNow }
        );
    }
}
