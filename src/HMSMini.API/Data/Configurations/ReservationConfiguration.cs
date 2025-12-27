using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Reservation entity
/// </summary>
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReservationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.ReservationNumber)
            .IsUnique();

        builder.Property(r => r.GuestName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.GuestEmail)
            .HasMaxLength(255);

        builder.Property(r => r.GuestMobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.SpecialRequests)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        // Indexes for common queries
        builder.HasIndex(r => r.CheckInDate);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.RoomId, r.CheckInDate, r.CheckOutDate });

        // Relationships
        builder.HasOne(r => r.Room)
            .WithMany()
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.CheckIn)
            .WithMany()
            .HasForeignKey(r => r.CheckInId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
