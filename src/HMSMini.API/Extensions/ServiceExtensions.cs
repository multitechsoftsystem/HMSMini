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
