using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using BCrypt.Net;

namespace HMSMini.API.Data;

/// <summary>
/// Initializes the database with seed data
/// </summary>
public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if data already exists
        if (context.RoomTypes.Any())
        {
            return; // Database has been seeded
        }

        // Seed Users
        var users = new User[]
        {
            new User
            {
                Username = "admin",
                Email = "admin@hmsmini.com",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "receptionist",
                Email = "reception@hmsmini.com",
                FullName = "Front Desk Receptionist",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Reception@123"),
                Role = UserRole.Receptionist,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "housekeeping",
                Email = "housekeeping@hmsmini.com",
                FullName = "Housekeeping Staff",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("House@123"),
                Role = UserRole.Housekeeping,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "maintenance",
                Email = "maintenance@hmsmini.com",
                FullName = "Maintenance Staff",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Maint@123"),
                Role = UserRole.Maintenance,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Room Types
        var roomTypes = new MRoomType[]
        {
            new MRoomType
            {
                RoomType = "Single",
                RoomDescription = "Single occupancy room with one bed, AC, and basic amenities"
            },
            new MRoomType
            {
                RoomType = "Double",
                RoomDescription = "Double occupancy room with two beds, AC, TV, and WiFi"
            },
            new MRoomType
            {
                RoomType = "Suite",
                RoomDescription = "Luxury suite with king-size bed, separate living area, premium amenities"
            },
            new MRoomType
            {
                RoomType = "Deluxe",
                RoomDescription = "Deluxe room with queen-size bed, mini-bar, city view, and premium services"
            }
        };

        context.RoomTypes.AddRange(roomTypes);
        context.SaveChanges();

        // Seed Sample Rooms
        var rooms = new RoomNo[]
        {
            // Single rooms
            new RoomNo { RoomNumber = "101", RoomTypeId = roomTypes[0].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "102", RoomTypeId = roomTypes[0].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "103", RoomTypeId = roomTypes[0].RoomTypeId, RoomStatus = RoomStatus.Available },

            // Double rooms
            new RoomNo { RoomNumber = "201", RoomTypeId = roomTypes[1].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "202", RoomTypeId = roomTypes[1].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "203", RoomTypeId = roomTypes[1].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "204", RoomTypeId = roomTypes[1].RoomTypeId, RoomStatus = RoomStatus.Available },

            // Suite rooms
            new RoomNo { RoomNumber = "301", RoomTypeId = roomTypes[2].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "302", RoomTypeId = roomTypes[2].RoomTypeId, RoomStatus = RoomStatus.Available },

            // Deluxe rooms
            new RoomNo { RoomNumber = "401", RoomTypeId = roomTypes[3].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "402", RoomTypeId = roomTypes[3].RoomTypeId, RoomStatus = RoomStatus.Available },
            new RoomNo { RoomNumber = "403", RoomTypeId = roomTypes[3].RoomTypeId, RoomStatus = RoomStatus.Maintenance,
                         RoomStatusFromDate = DateTime.Today,
                         RoomStatusToDate = DateTime.Today.AddDays(2) },
            new RoomNo { RoomNumber = "404", RoomTypeId = roomTypes[3].RoomTypeId, RoomStatus = RoomStatus.Dirty },
            new RoomNo { RoomNumber = "501", RoomTypeId = roomTypes[2].RoomTypeId, RoomStatus = RoomStatus.Blocked },
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        // Seed Business Sources
        var businessSources = new MBusinessSource[]
        {
            new MBusinessSource
            {
                SourceName = "Walk-In",
                Description = "Guest walked in directly to the hotel",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MBusinessSource
            {
                SourceName = "Online",
                Description = "Booking through online channels (website, OTA, etc.)",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MBusinessSource
            {
                SourceName = "Corporate",
                Description = "Corporate booking through company agreement",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MBusinessSource
            {
                SourceName = "Travel Agent",
                Description = "Booking through travel agent or agency",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MBusinessSource
            {
                SourceName = "Direct Call",
                Description = "Guest called hotel directly for reservation",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.BusinessSources.AddRange(businessSources);
        context.SaveChanges();

        // Seed Meal Plans
        var mealPlans = new MMealPlan[]
        {
            new MMealPlan
            {
                PlanCode = "EP",
                PlanName = "Room Only (European Plan)",
                Description = "Room only, no meals included",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MMealPlan
            {
                PlanCode = "CP",
                PlanName = "Breakfast Included (Continental Plan)",
                Description = "Room with breakfast",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MMealPlan
            {
                PlanCode = "MAP",
                PlanName = "Half Board (Modified American Plan)",
                Description = "Room with breakfast and dinner",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new MMealPlan
            {
                PlanCode = "AP",
                PlanName = "Full Board (American Plan)",
                Description = "Room with breakfast, lunch, and dinner",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.MealPlans.AddRange(mealPlans);
        context.SaveChanges();

        // Seed Tax Configuration (CGST + SGST for India)
        var taxConfigurations = new TaxConfiguration[]
        {
            new TaxConfiguration
            {
                TaxType = "CGST",
                TaxPercentage = 9.00m,
                ApplicableOn = "All",
                EffectiveFrom = new DateTime(2020, 1, 1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new TaxConfiguration
            {
                TaxType = "SGST",
                TaxPercentage = 9.00m,
                ApplicableOn = "All",
                EffectiveFrom = new DateTime(2020, 1, 1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.TaxConfigurations.AddRange(taxConfigurations);
        context.SaveChanges();

        // Seed System Settings (including Working Date)
        if (!context.SystemSettings.Any(s => s.SettingKey == "WorkingDate"))
        {
            var systemSettings = new SystemSetting[]
            {
                new SystemSetting
                {
                    SettingKey = "WorkingDate",
                    SettingValue = DateTime.Today.ToString("yyyy-MM-dd"),
                    DataType = "Date",
                    Description = "Current business/working date for hotel operations",
                    IsSystemLocked = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            context.SystemSettings.AddRange(systemSettings);
            context.SaveChanges();
        }
    }
}
