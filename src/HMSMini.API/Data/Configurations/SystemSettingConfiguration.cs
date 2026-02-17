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

        builder.HasData(new SystemSetting
        {
            Id = 2,
            SettingKey = "EnablePartialPayment",
            SettingValue = "true",
            DataType = "Bool",
            Description = "Enable or disable Partial Payment option in payment modal",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 3,
            SettingKey = "BillHeadingImagePath",
            SettingValue = "",
            DataType = "Image",
            Description = "Image displayed at the top of banquet bills and invoices",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 4,
            SettingKey = "HotelName",
            SettingValue = "",
            DataType = "String",
            Description = "Business name on invoice header",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 5,
            SettingKey = "HotelAddress",
            SettingValue = "",
            DataType = "String",
            Description = "Full address line",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 6,
            SettingKey = "HotelPhone",
            SettingValue = "",
            DataType = "String",
            Description = "Contact phone",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 7,
            SettingKey = "HotelEmail",
            SettingValue = "",
            DataType = "String",
            Description = "Contact email",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 8,
            SettingKey = "HotelGSTIN",
            SettingValue = "",
            DataType = "String",
            Description = "Supplier GSTIN",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 9,
            SettingKey = "HotelStateCode",
            SettingValue = "",
            DataType = "String",
            Description = "State code for GST (e.g., 27-Maharashtra)",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 10,
            SettingKey = "BankDetails",
            SettingValue = "",
            DataType = "String",
            Description = "Bank name, A/C, IFSC, Branch (single line)",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 11,
            SettingKey = "InvoiceTerms",
            SettingValue = "",
            DataType = "String",
            Description = "Terms & conditions text",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 12,
            SettingKey = "InvoiceFooterNote",
            SettingValue = "This is a computer generated invoice",
            DataType = "String",
            Description = "Footer disclaimer",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 13,
            SettingKey = "HotelSACCode",
            SettingValue = "996331",
            DataType = "String",
            Description = "Default SAC code for hospitality services",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 14,
            SettingKey = "DefaultBanquetTaxConfigId",
            SettingValue = "",
            DataType = "String",
            Description = "Default VoucherTaxConfig ID for banquet menus and charges",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        builder.HasData(new SystemSetting
        {
            Id = 15,
            SettingKey = "EnableRoomSharing",
            SettingValue = "false",
            DataType = "Bool",
            Description = "Enable or disable Room Sharing (room splitting) feature for multiple independent check-ins in the same room",
            IsSystemLocked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });
    }
}
