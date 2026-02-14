using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for CompanyTariff
/// </summary>
public class CompanyTariffConfiguration : IEntityTypeConfiguration<CompanyTariff>
{
    public void Configure(EntityTypeBuilder<CompanyTariff> builder)
    {
        // Table name
        builder.ToTable("CompanyTariff");

        // Primary key
        builder.HasKey(ct => ct.Id);

        // Properties
        builder.Property(ct => ct.CompanyId)
            .IsRequired();

        builder.Property(ct => ct.RoomTypeId)
            .IsRequired();

        builder.Property(ct => ct.OccupancyCount)
            .IsRequired();

        builder.Property(ct => ct.RatePerNight)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(ct => ct.EffectiveFrom)
            .IsRequired();

        builder.Property(ct => ct.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ct => ct.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint
        builder.HasIndex(ct => new { ct.CompanyId, ct.RoomTypeId, ct.OccupancyCount, ct.EffectiveFrom })
            .IsUnique();

        // Indexes
        builder.HasIndex(ct => ct.CompanyId);
        builder.HasIndex(ct => ct.RoomTypeId);
        builder.HasIndex(ct => new { ct.EffectiveFrom, ct.EffectiveTo });
        builder.HasIndex(ct => ct.IsActive);

        // Relationships
        builder.HasOne(ct => ct.Company)
            .WithMany(c => c.CompanyTariffs)
            .HasForeignKey(ct => ct.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ct => ct.RoomType)
            .WithMany()
            .HasForeignKey(ct => ct.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
