using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetBookingConfiguration : IEntityTypeConfiguration<BanquetBooking>
{
    public void Configure(EntityTypeBuilder<BanquetBooking> builder)
    {
        builder.ToTable("BanquetBookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.BookingNumber)
            .IsUnique()
            .HasDatabaseName("IX_BanquetBookings_BookingNumber");

        builder.Property(b => b.BanquetHallId)
            .IsRequired();

        builder.Property(b => b.EventTypeId)
            .IsRequired();

        builder.Property(b => b.EventDate)
            .IsRequired();

        builder.Property(b => b.EventStartTime)
            .IsRequired();

        builder.Property(b => b.EventEndTime)
            .IsRequired();

        builder.Property(b => b.ExpectedGuests)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired();

        builder.Property(b => b.PricingType)
            .IsRequired();

        builder.Property(b => b.ContactPersonName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.TaxType)
            .IsRequired();

        builder.Property(b => b.TaxSlabSnapshotJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(b => b.DiscountPercentage)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.Property(b => b.HallRent)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(b => b.Remarks)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100);

        builder.Property(b => b.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(b => b.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(b => b.BanquetHall)
            .WithMany(h => h.Bookings)
            .HasForeignKey(b => b.BanquetHallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.EventType)
            .WithMany(e => e.Bookings)
            .HasForeignKey(b => b.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Company)
            .WithMany()
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.CheckIn)
            .WithMany()
            .HasForeignKey(b => b.CheckInId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.EventDate)
            .HasDatabaseName("IX_BanquetBookings_EventDate");

        builder.HasIndex(b => b.BanquetHallId)
            .HasDatabaseName("IX_BanquetBookings_BanquetHallId");

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_BanquetBookings_Status");

        builder.HasIndex(b => b.DeletedAt)
            .HasDatabaseName("IX_BanquetBookings_DeletedAt");

        // Composite index for hall availability check
        builder.HasIndex(b => new { b.BanquetHallId, b.EventDate, b.Status })
            .HasDatabaseName("IX_BanquetBookings_Hall_Date_Status");
    }
}
