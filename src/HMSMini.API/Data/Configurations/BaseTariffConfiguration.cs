using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for BaseTariff
/// </summary>
public class BaseTariffConfiguration : IEntityTypeConfiguration<BaseTariff>
{
    public void Configure(EntityTypeBuilder<BaseTariff> builder)
    {
        // Table name
        builder.ToTable("BaseTariff");

        // Primary key
        builder.HasKey(bt => bt.Id);

        // Properties
        builder.Property(bt => bt.RoomTypeId)
            .IsRequired();

        builder.Property(bt => bt.OccupancyCount)
            .IsRequired();

        builder.Property(bt => bt.RatePerNight)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(bt => bt.EffectiveFrom)
            .IsRequired();

        builder.Property(bt => bt.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(bt => bt.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint
        builder.HasIndex(bt => new { bt.RoomTypeId, bt.OccupancyCount, bt.EffectiveFrom })
            .IsUnique();

        // Indexes
        builder.HasIndex(bt => bt.RoomTypeId);
        builder.HasIndex(bt => new { bt.EffectiveFrom, bt.EffectiveTo });
        builder.HasIndex(bt => bt.IsActive);

        // Relationships
        builder.HasOne(bt => bt.RoomType)
            .WithMany()
            .HasForeignKey(bt => bt.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
