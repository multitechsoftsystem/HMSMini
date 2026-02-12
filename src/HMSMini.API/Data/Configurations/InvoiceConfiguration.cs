using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_Invoices_InvoiceNumber");

        builder.Property(i => i.InvoiceDate)
            .IsRequired();

        builder.Property(i => i.CheckInId)
            .IsRequired();

        // One-to-one relationship with CheckIn
        builder.HasIndex(i => i.CheckInId)
            .IsUnique()
            .HasDatabaseName("IX_Invoices_CheckInId");

        builder.Property(i => i.RoomNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(i => i.GuestNames)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.CompanyName)
            .HasMaxLength(200);

        builder.Property(i => i.ActualCheckInDate)
            .IsRequired();

        builder.Property(i => i.ActualCheckOutDate)
            .IsRequired();

        builder.Property(i => i.TotalNights)
            .IsRequired();

        // JSON columns
        builder.Property(i => i.DailyChargesJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.AdditionalChargesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.TaxBreakdownJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Decimal columns
        builder.Property(i => i.RoomChargesSubtotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.MealChargesSubtotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.DiscountAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.AdditionalChargesSubtotal)
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
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0m);

        builder.Property(i => i.BalanceDue)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0m);

        builder.Property(i => i.PaymentStatus)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Unpaid");

        builder.Property(i => i.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(i => i.PaymentNotes)
            .HasMaxLength(1000);

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
        builder.HasOne(i => i.CheckIn)
            .WithOne()
            .HasForeignKey<Invoice>(i => i.CheckInId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.InvoiceDate)
            .HasDatabaseName("IX_Invoices_InvoiceDate");

        // Query filter for soft delete
        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
