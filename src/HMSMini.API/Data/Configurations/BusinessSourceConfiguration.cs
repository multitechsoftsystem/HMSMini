using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for MBusinessSource
/// </summary>
public class BusinessSourceConfiguration : IEntityTypeConfiguration<MBusinessSource>
{
    public void Configure(EntityTypeBuilder<MBusinessSource> builder)
    {
        // Table name
        builder.ToTable("MBusinessSources");

        // Primary key
        builder.HasKey(bs => bs.BusinessSourceId);

        // Properties
        builder.Property(bs => bs.SourceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bs => bs.Description)
            .HasMaxLength(500);

        builder.Property(bs => bs.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(bs => bs.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(bs => bs.CreatedBy)
            .HasMaxLength(100);

        builder.Property(bs => bs.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(bs => bs.DeletedBy)
            .HasMaxLength(100);

        // Unique constraint on SourceName
        builder.HasIndex(bs => bs.SourceName)
            .IsUnique();

        // Indexes
        builder.HasIndex(bs => bs.IsActive);
        builder.HasIndex(bs => bs.DeletedAt);

        // Relationships
        builder.HasMany(bs => bs.Reservations)
            .WithOne(r => r.BusinessSource)
            .HasForeignKey(r => r.BusinessSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(bs => bs.CheckIns)
            .WithOne(c => c.BusinessSource)
            .HasForeignKey(c => c.BusinessSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
