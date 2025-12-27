using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for CheckIn
/// </summary>
public class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        // Table name
        builder.ToTable("CheckIn");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.CheckInDate)
            .IsRequired();

        builder.Property(c => c.CheckOutDate)
            .IsRequired();

        builder.Property(c => c.ActualCheckOutDate)
            .IsRequired(false);

        builder.Property(c => c.Pax)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>(); // Store enum as int

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(c => c.CheckInDate);
        builder.HasIndex(c => c.CheckOutDate);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.RoomId, c.Status });

        // Relationships
        builder.HasOne(c => c.Room)
            .WithMany(r => r.CheckIns)
            .HasForeignKey(c => c.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Guests)
            .WithOne(g => g.CheckIn)
            .HasForeignKey(g => g.CheckInId)
            .OnDelete(DeleteBehavior.Cascade); // Delete guests when check-in is deleted
    }
}
