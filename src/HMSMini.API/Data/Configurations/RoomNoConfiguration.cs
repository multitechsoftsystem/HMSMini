using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for RoomNo
/// </summary>
public class RoomNoConfiguration : IEntityTypeConfiguration<RoomNo>
{
    public void Configure(EntityTypeBuilder<RoomNo> builder)
    {
        // Table name
        builder.ToTable("RoomNo");

        // Primary key
        builder.HasKey(r => r.RoomId);

        // Properties
        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.RoomStatus)
            .IsRequired()
            .HasConversion<int>(); // Store enum as int

        builder.Property(r => r.RoomStatusFromDate)
            .IsRequired(false);

        builder.Property(r => r.RoomStatusToDate)
            .IsRequired(false);

        // Unique constraint on RoomNumber
        builder.HasIndex(r => r.RoomNumber)
            .IsUnique();

        // Indexes for performance
        builder.HasIndex(r => r.RoomStatus);
        builder.HasIndex(r => new { r.RoomStatus, r.RoomStatusFromDate, r.RoomStatusToDate });

        // Relationships
        builder.HasOne(r => r.RoomType)
            .WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.CheckIns)
            .WithOne(c => c.Room)
            .HasForeignKey(c => c.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
