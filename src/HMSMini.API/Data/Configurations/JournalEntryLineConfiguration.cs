using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines", t =>
        {
            t.HasCheckConstraint("CK_JournalEntryLines_DebitAmount", "[DebitAmount] >= 0");
            t.HasCheckConstraint("CK_JournalEntryLines_CreditAmount", "[CreditAmount] >= 0");
        });

        builder.HasKey(l => l.Id);

        builder.Property(l => l.JournalEntryId).IsRequired();
        builder.Property(l => l.AccountId).IsRequired();

        builder.Property(l => l.DebitAmount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(l => l.CreditAmount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(l => l.Description).HasMaxLength(500);

        // Relationships
        builder.HasOne(l => l.JournalEntry)
            .WithMany(j => j.Lines)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Account)
            .WithMany()
            .HasForeignKey(l => l.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(l => l.JournalEntryId)
            .HasDatabaseName("IX_JournalEntryLines_JournalEntryId");

        builder.HasIndex(l => l.AccountId)
            .HasDatabaseName("IX_JournalEntryLines_AccountId");
    }
}
