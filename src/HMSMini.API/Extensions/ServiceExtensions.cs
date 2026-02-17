using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Services.Interfaces;
using HMSMini.API.Services.Implementations;

namespace HMSMini.API.Extensions;

/// <summary>
/// Extension methods for service registration
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Add application services to DI container
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ICheckInService, CheckInService>();
        services.AddScoped<IGuestService, GuestService>();
        services.AddScoped<IOcrService, OcrService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITariffService, TariffService>();
        services.AddScoped<IBusinessSourceService, BusinessSourceService>();
        services.AddScoped<IMealPlanService, MealPlanService>();
        services.AddScoped<IGuestTypeService, GuestTypeService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<ITaxService, TaxService>();

        // Banquet Module Services
        services.AddScoped<IBanquetHallService, BanquetHallService>();
        services.AddScoped<IEventTypeService, EventTypeService>();
        services.AddScoped<IBanquetServiceService, BanquetServiceService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IBanquetBookingService, BanquetBookingServiceImpl>();
        services.AddScoped<IBanquetChargeService, BanquetChargeService>();
        services.AddScoped<IBanquetPaymentService, BanquetPaymentService>();
        services.AddScoped<IBanquetBillingService, BanquetBillingService>();

        // Unified Payment & Financial Reports
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();

        // Day Closing and Voucher System Services
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        services.AddScoped<IVoucherService, VoucherService>();
        services.AddScoped<IDayClosingService, DayClosingService>();

        // Accounting Module Services
        services.AddScoped<IFinancialYearService, FinancialYearService>();
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<IExpenseHeadService, ExpenseHeadService>();
        services.AddScoped<IExpenseVoucherService, ExpenseVoucherService>();
        services.AddScoped<IPaymentVoucherService, PaymentVoucherService>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<IAccountingReportService, AccountingReportService>();

        return services;
    }

    /// <summary>
    /// Add database services to DI container
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
