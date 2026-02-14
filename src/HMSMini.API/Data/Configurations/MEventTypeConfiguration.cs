using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MEventTypeConfiguration : IEntityTypeConfiguration<MEventType>
{
    public void Configure(EntityTypeBuilder<MEventType> builder)
    {
        builder.ToTable("MEventTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventTypeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(e => e.EventTypeName)
            .HasDatabaseName("IX_MEventTypes_EventTypeName");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_MEventTypes_IsActive");

        builder.HasIndex(e => e.DeletedAt)
            .HasDatabaseName("IX_MEventTypes_DeletedAt");
    }
}
