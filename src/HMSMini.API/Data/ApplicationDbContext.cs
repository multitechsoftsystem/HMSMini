using Microsoft.EntityFrameworkCore;
using HMSMini.API.Models.Entities;
using HMSMini.API.Data.Configurations;

namespace HMSMini.API.Data;

/// <summary>
/// Database context for MSS application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<MRoomType> RoomTypes { get; set; } = null!;
    public DbSet<RoomNo> Rooms { get; set; } = null!;
    public DbSet<CheckIn> CheckIns { get; set; } = null!;
    public DbSet<Guest> Guests { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<BaseTariff> BaseTariffs { get; set; } = null!;
    public DbSet<CompanyTariff> CompanyTariffs { get; set; } = null!;
    public DbSet<MBusinessSource> BusinessSources { get; set; } = null!;
    public DbSet<MMealPlan> MealPlans { get; set; } = null!;
    public DbSet<MealPlanRate> MealPlanRates { get; set; } = null!;
    public DbSet<MGuestType> MGuestTypes { get; set; } = null!;
    public DbSet<AdditionalCharge> AdditionalCharges { get; set; } = null!;
    public DbSet<TaxConfiguration> TaxConfigurations { get; set; } = null!;
    public DbSet<TaxSlab> TaxSlabs { get; set; } = null!;
    public DbSet<VoucherTaxConfiguration> VoucherTaxConfigurations { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<SystemSetting> SystemSettings { get; set; } = null!;
    public DbSet<Voucher> Vouchers { get; set; } = null!;
    public DbSet<DayClosingAudit> DayClosingAudits { get; set; } = null!;

    // Banquet Module
    public DbSet<MBanquetHall> BanquetHalls { get; set; } = null!;
    public DbSet<MEventType> EventTypes { get; set; } = null!;
    public DbSet<MBanquetService> BanquetServices { get; set; } = null!;
    public DbSet<MMenuCategory> MenuCategories { get; set; } = null!;
    public DbSet<MMenuItem> MenuItems { get; set; } = null!;
    public DbSet<MMenuPackage> MenuPackages { get; set; } = null!;
    public DbSet<MMenuPackageItem> MenuPackageItems { get; set; } = null!;
    public DbSet<BanquetBooking> BanquetBookings { get; set; } = null!;
    public DbSet<BanquetBookingMenu> BanquetBookingMenus { get; set; } = null!;
    public DbSet<BanquetBookingService> BanquetBookingServices { get; set; } = null!;
    public DbSet<BanquetCharge> BanquetCharges { get; set; } = null!;
    public DbSet<BanquetPayment> BanquetPayments { get; set; } = null!;
    public DbSet<BanquetInvoice> BanquetInvoices { get; set; } = null!;

    // Unified Payment System
    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new RoomTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoomNoConfiguration());
        modelBuilder.ApplyConfiguration(new CheckInConfiguration());
        modelBuilder.ApplyConfiguration(new GuestConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ReservationConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new BaseTariffConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyTariffConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessSourceConfiguration());
        modelBuilder.ApplyConfiguration(new MealPlanConfiguration());
        modelBuilder.ApplyConfiguration(new MealPlanRateConfiguration());
        modelBuilder.ApplyConfiguration(new GuestTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AdditionalChargeConfiguration());
        modelBuilder.ApplyConfiguration(new TaxConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new TaxSlabConfiguration());
        modelBuilder.ApplyConfiguration(new VoucherTaxConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new SystemSettingConfiguration());
        modelBuilder.ApplyConfiguration(new VoucherConfiguration());
        modelBuilder.ApplyConfiguration(new DayClosingAuditConfiguration());

        // Banquet Module
        modelBuilder.ApplyConfiguration(new MBanquetHallConfiguration());
        modelBuilder.ApplyConfiguration(new MEventTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MBanquetServiceConfiguration());
        modelBuilder.ApplyConfiguration(new MMenuCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new MMenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new MMenuPackageConfiguration());
        modelBuilder.ApplyConfiguration(new MMenuPackageItemConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetBookingConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetBookingMenuConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetBookingServiceConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetChargeConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new BanquetInvoiceConfiguration());

        // Unified Payment System
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for audit fields
        var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is CheckIn || e.Entity is Reservation ||
                        e.Entity is Company || e.Entity is BaseTariff || e.Entity is CompanyTariff ||
                        e.Entity is MBusinessSource || e.Entity is MMealPlan || e.Entity is MealPlanRate ||
                        e.Entity is MGuestType || e.Entity is AdditionalCharge || e.Entity is TaxConfiguration ||
                        e.Entity is TaxSlab || e.Entity is VoucherTaxConfiguration || e.Entity is Invoice ||
                        e.Entity is SystemSetting || e.Entity is Voucher || e.Entity is DayClosingAudit ||
                        e.Entity is MBanquetHall || e.Entity is MEventType || e.Entity is MBanquetService ||
                        e.Entity is MMenuCategory || e.Entity is MMenuItem || e.Entity is MMenuPackage ||
                        e.Entity is BanquetBooking || e.Entity is BanquetBookingMenu ||
                        e.Entity is BanquetBookingService || e.Entity is BanquetCharge ||
                        e.Entity is BanquetPayment || e.Entity is BanquetInvoice ||
                        e.Entity is Payment) &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is CheckIn checkIn)
            {
                if (entry.State == EntityState.Added)
                {
                    checkIn.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    checkIn.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Reservation reservation)
            {
                if (entry.State == EntityState.Added)
                {
                    reservation.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    reservation.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Company company)
            {
                if (entry.State == EntityState.Added)
                {
                    company.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    company.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is BaseTariff baseTariff)
            {
                if (entry.State == EntityState.Added)
                {
                    baseTariff.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    baseTariff.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is CompanyTariff companyTariff)
            {
                if (entry.State == EntityState.Added)
                {
                    companyTariff.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    companyTariff.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is MBusinessSource businessSource)
            {
                if (entry.State == EntityState.Added)
                {
                    businessSource.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    businessSource.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is MMealPlan mealPlan)
            {
                if (entry.State == EntityState.Added)
                {
                    mealPlan.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    mealPlan.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is MealPlanRate mealPlanRate)
            {
                if (entry.State == EntityState.Added)
                {
                    mealPlanRate.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    mealPlanRate.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is MGuestType guestType)
            {
                if (entry.State == EntityState.Added)
                {
                    guestType.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    guestType.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is AdditionalCharge additionalCharge)
            {
                if (entry.State == EntityState.Added)
                {
                    additionalCharge.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    additionalCharge.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is TaxConfiguration taxConfig)
            {
                if (entry.State == EntityState.Added)
                {
                    taxConfig.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    taxConfig.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is TaxSlab taxSlab)
            {
                if (entry.State == EntityState.Added)
                {
                    taxSlab.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    taxSlab.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is VoucherTaxConfiguration voucherTaxConfig)
            {
                if (entry.State == EntityState.Added)
                {
                    voucherTaxConfig.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    voucherTaxConfig.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Invoice invoice)
            {
                if (entry.State == EntityState.Added)
                {
                    invoice.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    invoice.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is SystemSetting systemSetting)
            {
                if (entry.State == EntityState.Added)
                {
                    systemSetting.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    systemSetting.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Voucher voucher)
            {
                if (entry.State == EntityState.Added)
                {
                    voucher.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    voucher.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is DayClosingAudit dayClosingAudit)
            {
                if (entry.State == EntityState.Added)
                {
                    dayClosingAudit.ClosedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is MBanquetHall banquetHall)
            {
                if (entry.State == EntityState.Added)
                    banquetHall.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetHall.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is MEventType eventType)
            {
                if (entry.State == EntityState.Added)
                    eventType.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    eventType.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is MBanquetService banquetService)
            {
                if (entry.State == EntityState.Added)
                    banquetService.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetService.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is MMenuCategory menuCategory)
            {
                if (entry.State == EntityState.Added)
                    menuCategory.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    menuCategory.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is MMenuItem menuItem)
            {
                if (entry.State == EntityState.Added)
                    menuItem.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    menuItem.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is MMenuPackage menuPackage)
            {
                if (entry.State == EntityState.Added)
                    menuPackage.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    menuPackage.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetBooking banquetBooking)
            {
                if (entry.State == EntityState.Added)
                    banquetBooking.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetBooking.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetBookingMenu bookingMenu)
            {
                if (entry.State == EntityState.Added)
                    bookingMenu.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    bookingMenu.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetBookingService bookingService)
            {
                if (entry.State == EntityState.Added)
                    bookingService.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    bookingService.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetCharge banquetCharge)
            {
                if (entry.State == EntityState.Added)
                    banquetCharge.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetCharge.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetPayment banquetPayment)
            {
                if (entry.State == EntityState.Added)
                    banquetPayment.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetPayment.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BanquetInvoice banquetInvoice)
            {
                if (entry.State == EntityState.Added)
                    banquetInvoice.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    banquetInvoice.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Payment payment)
            {
                if (entry.State == EntityState.Added)
                    payment.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    payment.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
