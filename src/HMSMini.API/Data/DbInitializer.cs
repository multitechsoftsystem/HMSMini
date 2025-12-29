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
            new RoomNo { RoomNumber = "501", RoomTypeId = roomTypes[2].RoomTypeId, RoomStatus = RoomStatus.Management },
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}
