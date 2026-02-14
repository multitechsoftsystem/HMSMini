using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for Company
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table name
        builder.ToTable("Company");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .HasMaxLength(100);

        builder.Property(c => c.GSTNumber)
            .HasMaxLength(50);

        builder.Property(c => c.ContactPerson)
            .HasMaxLength(200);

        builder.Property(c => c.Designation)
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.ContactNumber)
            .HasMaxLength(20);

        builder.Property(c => c.DiscountPercentage)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(c => c.CompanyName);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.DeletedAt);

        // Unique index on GST Number (only for non-deleted records with GST)
        builder.HasIndex(c => c.GSTNumber)
            .IsUnique()
            .HasFilter("[GSTNumber] IS NOT NULL AND [DeletedAt] IS NULL")
            .HasDatabaseName("IX_Company_GSTNumber_Unique");

        // Relationships
        builder.HasMany(c => c.CompanyTariffs)
            .WithOne(ct => ct.Company)
            .HasForeignKey(ct => ct.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CheckIns)
            .WithOne(ci => ci.Company)
            .HasForeignKey(ci => ci.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Reservations)
            .WithOne(r => r.Company)
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
