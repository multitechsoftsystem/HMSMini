using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for MMealPlan
/// </summary>
public class MealPlanConfiguration : IEntityTypeConfiguration<MMealPlan>
{
    public void Configure(EntityTypeBuilder<MMealPlan> builder)
    {
        builder.ToTable("MMealPlans");

        builder.HasKey(mp => mp.MealPlanId);

        builder.Property(mp => mp.PlanCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(mp => mp.PlanName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mp => mp.Description)
            .HasMaxLength(500);

        builder.Property(mp => mp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mp => mp.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(mp => mp.CreatedBy)
            .HasMaxLength(100);

        builder.Property(mp => mp.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(mp => mp.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(mp => mp.PlanCode)
            .IsUnique()
            .HasDatabaseName("IX_MMealPlans_PlanCode");

        builder.HasIndex(mp => mp.IsActive)
            .HasDatabaseName("IX_MMealPlans_IsActive");

        builder.HasIndex(mp => mp.DeletedAt)
            .HasDatabaseName("IX_MMealPlans_DeletedAt");

        // Relationships
        builder.HasMany(mp => mp.MealPlanRates)
            .WithOne(mpr => mpr.MealPlan)
            .HasForeignKey(mpr => mpr.MealPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mp => mp.Reservations)
            .WithOne(r => r.MealPlan)
            .HasForeignKey(r => r.MealPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mp => mp.CheckIns)
            .WithOne(c => c.MealPlan)
            .HasForeignKey(c => c.MealPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
