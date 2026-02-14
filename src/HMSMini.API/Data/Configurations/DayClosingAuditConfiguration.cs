using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class DayClosingAuditConfiguration : IEntityTypeConfiguration<DayClosingAudit>
{
    public void Configure(EntityTypeBuilder<DayClosingAudit> builder)
    {
        builder.ToTable("DayClosingAudit");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.ClosedDate)
            .IsRequired();

        builder.HasIndex(d => d.ClosedDate)
            .IsUnique()
            .HasDatabaseName("IX_DayClosingAudit_ClosedDate");

        builder.Property(d => d.NextWorkingDate)
            .IsRequired();

        builder.Property(d => d.TotalActiveCheckIns)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.TotalVouchersPosted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.TotalRevenuePosted)
            .IsRequired()
            .HasColumnType("decimal(12,2)")
            .HasDefaultValue(0);

        builder.Property(d => d.VoucherSummaryJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.CheckInSummaryJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.ClosingStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Completed");

        builder.Property(d => d.ErrorLog)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.ClosedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.ClosedBy)
            .HasMaxLength(100);

        builder.Property(d => d.DurationSeconds);
    }
}
