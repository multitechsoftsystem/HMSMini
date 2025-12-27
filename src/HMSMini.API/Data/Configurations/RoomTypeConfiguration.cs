using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for MRoomType
/// </summary>
public class RoomTypeConfiguration : IEntityTypeConfiguration<MRoomType>
{
    public void Configure(EntityTypeBuilder<MRoomType> builder)
    {
        // Table name
        builder.ToTable("MRoomTypes");

        // Primary key
        builder.HasKey(rt => rt.RoomTypeId);

        // Properties
        builder.Property(rt => rt.RoomType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(rt => rt.RoomDescription)
            .HasMaxLength(500);

        // Unique constraint on RoomType
        builder.HasIndex(rt => rt.RoomType)
            .IsUnique();

        // Relationships
        builder.HasMany(rt => rt.Rooms)
            .WithOne(r => r.RoomType)
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
