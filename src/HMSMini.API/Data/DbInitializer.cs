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
            // Still seed accounting data if missing (added via migration after initial seed)
            SeedAccountingData(context);
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
                Role = UserRole.Admin,
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
            },
            new User
            {
                Username = "developer",
                Email = "developer@hmsmini.com",
                FullName = "Developer Support",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dev@123"),
                Role = UserRole.Developer,
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

        // Seed Accounting Data
        SeedAccountingData(context);
    }

    private static void SeedAccountingData(ApplicationDbContext context)
    {
        // Seed Financial Year (auto-detect current FY: April-March Indian standard)
        if (!context.FinancialYears.Any())
        {
            var today = DateTime.Today;
            int fyStartYear = today.Month >= 4 ? today.Year : today.Year - 1;
            var fy = new FinancialYear
            {
                Name = $"{fyStartYear}-{(fyStartYear + 1) % 100:D2}",
                StartDate = new DateTime(fyStartYear, 4, 1),
                EndDate = new DateTime(fyStartYear + 1, 3, 31),
                IsCurrent = true,
                IsClosed = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.FinancialYears.Add(fy);
            context.SaveChanges();
        }

        // Seed Chart of Accounts
        if (!context.ChartOfAccounts.Any())
        {
            var accounts = new ChartOfAccount[]
            {
                // Assets
                new() { AccountCode = "1001", AccountName = "Cash in Hand", AccountType = AccountType.Asset, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "1002", AccountName = "Bank Account", AccountType = AccountType.Asset, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "1003", AccountName = "Accounts Receivable", AccountType = AccountType.Asset, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },

                // Liabilities
                new() { AccountCode = "2001", AccountName = "Accounts Payable", AccountType = AccountType.Liability, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "2002", AccountName = "CGST Payable", AccountType = AccountType.Liability, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "2003", AccountName = "SGST Payable", AccountType = AccountType.Liability, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "2004", AccountName = "IGST Payable", AccountType = AccountType.Liability, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "2005", AccountName = "Advance from Guests", AccountType = AccountType.Liability, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },

                // Equity
                new() { AccountCode = "3001", AccountName = "Capital Account", AccountType = AccountType.Equity, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },

                // Income
                new() { AccountCode = "4001", AccountName = "Room Revenue", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4002", AccountName = "Meal Plan Revenue", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4003", AccountName = "Banquet Hall Rent", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4004", AccountName = "Banquet Menu Revenue", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4005", AccountName = "Banquet Service Revenue", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4006", AccountName = "Additional Charges Revenue", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "4007", AccountName = "Discount Given", AccountType = AccountType.Income, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },

                // Expenses
                new() { AccountCode = "5001", AccountName = "Electricity Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5002", AccountName = "Water Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5003", AccountName = "Salary Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5004", AccountName = "Maintenance Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5005", AccountName = "Housekeeping Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5006", AccountName = "Food & Beverage Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5007", AccountName = "Laundry Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5008", AccountName = "Telephone Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5009", AccountName = "Internet Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5010", AccountName = "Rent Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
                new() { AccountCode = "5099", AccountName = "Miscellaneous Expense", AccountType = AccountType.Expense, IsSystemAccount = true, IsActive = true, CreatedBy = "System" },
            };

            context.ChartOfAccounts.AddRange(accounts);
            context.SaveChanges();
        }

        // Seed Expense Heads mapped to GL accounts
        if (!context.ExpenseHeads.Any())
        {
            var accountMap = context.ChartOfAccounts
                .Where(a => a.AccountType == AccountType.Expense)
                .ToDictionary(a => a.AccountCode, a => a.Id);

            var expenseHeads = new MExpenseHead[]
            {
                new() { Name = "Electricity", DefaultAccountId = accountMap.GetValueOrDefault("5001"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Water", DefaultAccountId = accountMap.GetValueOrDefault("5002"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Salary", DefaultAccountId = accountMap.GetValueOrDefault("5003"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Maintenance", DefaultAccountId = accountMap.GetValueOrDefault("5004"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Housekeeping", DefaultAccountId = accountMap.GetValueOrDefault("5005"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Food & Beverage", DefaultAccountId = accountMap.GetValueOrDefault("5006"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Laundry", DefaultAccountId = accountMap.GetValueOrDefault("5007"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Telephone", DefaultAccountId = accountMap.GetValueOrDefault("5008"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Internet", DefaultAccountId = accountMap.GetValueOrDefault("5009"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Rent", DefaultAccountId = accountMap.GetValueOrDefault("5010"), IsActive = true, CreatedBy = "System" },
                new() { Name = "Miscellaneous", DefaultAccountId = accountMap.GetValueOrDefault("5099"), IsActive = true, CreatedBy = "System" },
            };

            context.ExpenseHeads.AddRange(expenseHeads);
            context.SaveChanges();
        }
    }
}
