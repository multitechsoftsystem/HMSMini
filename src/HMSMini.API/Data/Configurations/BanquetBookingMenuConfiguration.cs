using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetBookingMenuConfiguration : IEntityTypeConfiguration<BanquetBookingMenu>
{
    public void Configure(EntityTypeBuilder<BanquetBookingMenu> builder)
    {
        builder.ToTable("BanquetBookingMenus");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.BanquetBookingId)
            .IsRequired();

        builder.Property(m => m.ItemName)
            .HasMaxLength(200);

        builder.Property(m => m.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(m => m.RatePerPlate)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(m => m.TotalAmount)
            .HasComputedColumnSql("[RatePerPlate] * [Quantity]", stored: true)
            .HasColumnType("decimal(10,2)");

        builder.Property(m => m.ApplyTax)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(100);

        builder.Property(m => m.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(m => m.DeletedBy)
            .HasMaxLength(100);

        // Foreign keys
        builder.HasOne(m => m.BanquetBooking)
            .WithMany(b => b.BookingMenus)
            .HasForeignKey(m => m.BanquetBookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.MenuPackage)
            .WithMany()
            .HasForeignKey(m => m.MenuPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.MenuItem)
            .WithMany()
            .HasForeignKey(m => m.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.VoucherTaxConfig)
            .WithMany()
            .HasForeignKey(m => m.VoucherTaxConfigId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(m => m.BanquetBookingId)
            .HasDatabaseName("IX_BanquetBookingMenus_BanquetBookingId");

        // Query filter for soft delete
        builder.HasQueryFilter(m => m.DeletedAt == null);
    }
}
