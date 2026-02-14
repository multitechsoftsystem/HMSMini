using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SettingKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.SettingKey)
            .IsUnique()
            .HasDatabaseName("IX_SystemSettings_SettingKey");

        builder.Property(s => s.SettingValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.DataType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.IsSystemLocked)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.UpdatedBy)
            .HasMaxLength(100);

        // Seed the WorkingDate setting with today's date
        builder.HasData(new SystemSetting
        {
            Id = 1,
            SettingKey = "WorkingDate",
            SettingValue = DateTime.Today.ToString("yyyy-MM-dd"),
            DataType = "Date",
            Description = "Current business/working date for hotel operations",
            IsSystemLocked = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });
    }
}
