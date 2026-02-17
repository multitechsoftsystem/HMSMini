using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.ToTable("ChartOfAccounts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.AccountCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.AccountCode)
            .IsUnique()
            .HasDatabaseName("IX_ChartOfAccounts_AccountCode");

        builder.Property(c => c.AccountName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.AccountType).IsRequired();

        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100);
        builder.Property(c => c.DeletedBy).HasMaxLength(100);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Self-referencing hierarchy
        builder.HasOne(c => c.ParentAccount)
            .WithMany(c => c.ChildAccounts)
            .HasForeignKey(c => c.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.AccountType)
            .HasDatabaseName("IX_ChartOfAccounts_AccountType");

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
