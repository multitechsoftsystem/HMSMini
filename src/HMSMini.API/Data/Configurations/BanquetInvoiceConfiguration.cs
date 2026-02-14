using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class BanquetInvoiceConfiguration : IEntityTypeConfiguration<BanquetInvoice>
{
    public void Configure(EntityTypeBuilder<BanquetInvoice> builder)
    {
        builder.ToTable("BanquetInvoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_BanquetInvoices_InvoiceNumber");

        builder.Property(i => i.InvoiceDate)
            .IsRequired();

        builder.Property(i => i.BanquetBookingId)
            .IsRequired();

        builder.HasIndex(i => i.BanquetBookingId)
            .IsUnique()
            .HasDatabaseName("IX_BanquetInvoices_BanquetBookingId");

        // Snapshot fields
        builder.Property(i => i.HallName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.EventTypeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.EventDate)
            .IsRequired();

        builder.Property(i => i.ContactPersonName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.CompanyName)
            .HasMaxLength(200);

        // JSON columns
        builder.Property(i => i.MenuChargesJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.ServiceChargesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.AdditionalChargesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.TaxBreakdownJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.PaymentHistoryJson)
            .HasColumnType("nvarchar(max)");

        // Decimal columns
        builder.Property(i => i.HallRent)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.MenuChargesSubtotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.ServiceChargesSubtotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.AdditionalChargesSubtotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.DiscountAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.SubtotalBeforeTax)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.TotalTax)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.GrandTotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.TotalPaid)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.BalanceDue)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.PaymentStatus)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Unpaid");

        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.DeletedBy)
            .HasMaxLength(100);

        // Foreign key
        builder.HasOne(i => i.BanquetBooking)
            .WithOne()
            .HasForeignKey<BanquetInvoice>(i => i.BanquetBookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.InvoiceDate)
            .HasDatabaseName("IX_BanquetInvoices_InvoiceDate");

        // Query filter for soft delete
        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
