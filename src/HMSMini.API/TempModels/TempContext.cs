using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HMSMini.API.TempModels;

public partial class TempContext : DbContext
{
    public TempContext()
    {
    }

    public TempContext(DbContextOptions<TempContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CheckIn> CheckIns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.ToTable("CheckIn");

            entity.HasIndex(e => e.BusinessSourceId, "IX_CheckIn_BusinessSourceId");

            entity.HasIndex(e => e.CheckInDate, "IX_CheckIn_CheckInDate");

            entity.HasIndex(e => e.CheckOutDate, "IX_CheckIn_CheckOutDate");

            entity.HasIndex(e => e.CompanyId, "IX_CheckIn_CompanyId");

            entity.HasIndex(e => e.GuestTypeId, "IX_CheckIn_GuestTypeId");

            entity.HasIndex(e => e.MealPlanId, "IX_CheckIn_MealPlanId");

            entity.HasIndex(e => new { e.RoomId, e.Status }, "IX_CheckIn_RoomId_Status");

            entity.HasIndex(e => e.Status, "IX_CheckIn_Status");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MealPlanRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RegistrationNo).HasMaxLength(50);
            entity.Property(e => e.Remarks).HasMaxLength(1000);
            entity.Property(e => e.TariffApplied).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
