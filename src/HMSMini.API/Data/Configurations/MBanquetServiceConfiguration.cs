using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MBanquetServiceConfiguration : IEntityTypeConfiguration<MBanquetService>
{
    public void Configure(EntityTypeBuilder<MBanquetService> builder)
    {
        builder.ToTable("MBanquetServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ServiceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.DefaultRate)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.Unit)
            .HasMaxLength(50);

        builder.Property(s => s.ApplyTax)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.IsActive)
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
        builder.HasOne(s => s.VoucherTaxConfig)
            .WithMany()
            .HasForeignKey(s => s.VoucherTaxConfigId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.ServiceName)
            .HasDatabaseName("IX_MBanquetServices_ServiceName");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_MBanquetServices_IsActive");

        builder.HasIndex(s => s.DeletedAt)
            .HasDatabaseName("IX_MBanquetServices_DeletedAt");
    }
}
