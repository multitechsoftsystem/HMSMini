using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Data.Configurations;

/// <summary>
/// Entity configuration for MealPlanRate
/// </summary>
public class MealPlanRateConfiguration : IEntityTypeConfiguration<MealPlanRate>
{
    public void Configure(EntityTypeBuilder<MealPlanRate> builder)
    {
        builder.ToTable("MealPlanRates");

        builder.HasKey(mpr => mpr.Id);

        builder.Property(mpr => mpr.MealPlanId)
            .IsRequired();

        builder.Property(mpr => mpr.RoomTypeId)
            .IsRequired();

        builder.Property(mpr => mpr.RatePerPersonPerNight)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(mpr => mpr.EffectiveFrom)
            .IsRequired();

        builder.Property(mpr => mpr.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mpr => mpr.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(mpr => mpr.CreatedBy)
            .HasMaxLength(100);

        builder.Property(mpr => mpr.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(mpr => mpr.DeletedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(mpr => new { mpr.MealPlanId, mpr.RoomTypeId, mpr.EffectiveFrom })
            .HasDatabaseName("IX_MealPlanRates_MealPlan_RoomType_EffectiveFrom");

        builder.HasIndex(mpr => mpr.IsActive)
            .HasDatabaseName("IX_MealPlanRates_IsActive");

        builder.HasIndex(mpr => mpr.DeletedAt)
            .HasDatabaseName("IX_MealPlanRates_DeletedAt");

        // Relationships
        builder.HasOne(mpr => mpr.MealPlan)
            .WithMany(mp => mp.MealPlanRates)
            .HasForeignKey(mpr => mpr.MealPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mpr => mpr.RoomType)
            .WithMany()
            .HasForeignKey(mpr => mpr.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
