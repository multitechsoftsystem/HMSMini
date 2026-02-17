using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.EntryNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(j => j.EntryNumber)
            .IsUnique()
            .HasDatabaseName("IX_JournalEntries_EntryNumber");

        builder.Property(j => j.EntryDate).IsRequired();
        builder.Property(j => j.FinancialYearId).IsRequired();
        builder.Property(j => j.SourceType).IsRequired();

        builder.Property(j => j.Description).HasMaxLength(500);

        builder.Property(j => j.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(j => j.CreatedBy).HasMaxLength(100);
        builder.Property(j => j.UpdatedBy).HasMaxLength(100);
        builder.Property(j => j.DeletedBy).HasMaxLength(100);

        builder.Property(j => j.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(j => j.FinancialYear)
            .WithMany()
            .HasForeignKey(j => j.FinancialYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.ReversalOf)
            .WithMany()
            .HasForeignKey(j => j.ReversalOfId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(j => j.EntryDate)
            .HasDatabaseName("IX_JournalEntries_EntryDate");

        builder.HasIndex(j => j.FinancialYearId)
            .HasDatabaseName("IX_JournalEntries_FinancialYearId");

        builder.HasIndex(j => new { j.SourceType, j.SourceId })
            .HasDatabaseName("IX_JournalEntries_Source");

        builder.HasQueryFilter(j => j.DeletedAt == null);
    }
}
