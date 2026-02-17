using HMSMini.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSMini.API.Data.Configurations;

public class MExpenseHeadConfiguration : IEntityTypeConfiguration<MExpenseHead>
{
    public void Configure(EntityTypeBuilder<MExpenseHead> builder)
    {
        builder.ToTable("MExpenseHeads");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_MExpenseHeads_Name");

        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);
        builder.Property(e => e.DeletedBy).HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.DefaultAccount)
            .WithMany()
            .HasForeignKey(e => e.DefaultAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
