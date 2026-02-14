using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetBookingServiceConfiguration : IEntityTypeConfiguration<BanquetBookingService>
{
    public void Configure(EntityTypeBuilder<BanquetBookingService> builder)
    {
        builder.ToTable("BanquetBookingServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BanquetBookingId)
            .IsRequired();

        builder.Property(s => s.ServiceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.Rate)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.TotalAmount)
            .HasComputedColumnSql("[Rate] * [Quantity]", stored: true)
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.ApplyTax)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(s => s.BanquetBooking)
            .WithMany(b => b.BookingServices)
            .HasForeignKey(s => s.BanquetBookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.BanquetService)
            .WithMany()
            .HasForeignKey(s => s.BanquetServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.VoucherTaxConfig)
            .WithMany()
            .HasForeignKey(s => s.VoucherTaxConfigId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.BanquetBookingId)
            .HasDatabaseName("IX_BanquetBookingServices_BanquetBookingId");

        // Query filter for soft delete
        builder.HasQueryFilter(s => s.DeletedAt == null);
    }
}
