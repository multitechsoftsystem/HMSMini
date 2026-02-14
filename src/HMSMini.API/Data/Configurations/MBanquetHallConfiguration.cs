using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MBanquetHallConfiguration : IEntityTypeConfiguration<MBanquetHall>
{
    public void Configure(EntityTypeBuilder<MBanquetHall> builder)
    {
        builder.ToTable("MBanquetHalls");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.HallName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.MaxCapacity)
            .IsRequired();

        builder.Property(h => h.MinCapacity)
            .HasDefaultValue(0);

        builder.Property(h => h.RentPerEvent)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(h => h.Location)
            .HasMaxLength(200);

        builder.Property(h => h.Features)
            .HasMaxLength(1000);

        builder.Property(h => h.ImagePath)
            .HasMaxLength(500);

        builder.Property(h => h.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(h => h.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(h => h.CreatedBy)
            .HasMaxLength(100);

        builder.Property(h => h.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(h => h.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(h => h.HallName)
            .HasDatabaseName("IX_MBanquetHalls_HallName");

        builder.HasIndex(h => h.IsActive)
            .HasDatabaseName("IX_MBanquetHalls_IsActive");

        builder.HasIndex(h => h.DeletedAt)
            .HasDatabaseName("IX_MBanquetHalls_DeletedAt");
    }
}
