using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for Guest
/// </summary>
public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        // Table name
        builder.ToTable("Guest");

        // Primary key
        builder.HasKey(g => g.Id);

        // Properties
        builder.Property(g => g.GuestNumber)
            .IsRequired();

        builder.Property(g => g.GuestName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Address)
            .HasMaxLength(500);

        builder.Property(g => g.City)
            .HasMaxLength(100);

        builder.Property(g => g.State)
            .HasMaxLength(100);

        builder.Property(g => g.Country)
            .HasMaxLength(100);

        builder.Property(g => g.MobileNo)
            .HasMaxLength(20);

        builder.Property(g => g.Photo1Path)
            .HasMaxLength(500);

        builder.Property(g => g.Photo2Path)
            .HasMaxLength(500);

        // Composite index on CheckInId and GuestNumber
        builder.HasIndex(g => new { g.CheckInId, g.GuestNumber })
            .IsUnique(); // Ensure guest numbers are unique within a check-in

        // Relationships
        builder.HasOne(g => g.CheckIn)
            .WithMany(c => c.Guests)
            .HasForeignKey(g => g.CheckInId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
