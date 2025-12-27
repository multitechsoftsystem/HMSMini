using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Implementations;

namespace HMSMini.Tests.Services;

public class RoomServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RoomService _roomService;
    private readonly Mock<ILogger<RoomService>> _loggerMock;

    public RoomServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<RoomService>>();
        _roomService = new RoomService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var roomType = new MRoomType
        {
            RoomTypeId = 1,
            RoomType = "Single",
            RoomDescription = "Single room"
        };

        _context.RoomTypes.Add(roomType);

        var rooms = new List<RoomNo>
        {
            new RoomNo
            {
                RoomId = 1,
                RoomNumber = "101",
                RoomTypeId = 1,
                RoomStatus = RoomStatus.Available
            },
            new RoomNo
            {
                RoomId = 2,
                RoomNumber = "102",
                RoomTypeId = 1,
                RoomStatus = RoomStatus.Occupied,
                RoomStatusFromDate = DateTime.Today,
                RoomStatusToDate = DateTime.Today.AddDays(2)
            },
            new RoomNo
            {
                RoomId = 3,
                RoomNumber = "103",
                RoomTypeId = 1,
                RoomStatus = RoomStatus.Maintenance,
                RoomStatusFromDate = DateTime.Today,
                RoomStatusToDate = DateTime.Today.AddDays(1)
            },
            new RoomNo
            {
                RoomId = 4,
                RoomNumber = "104",
                RoomTypeId = 1,
                RoomStatus = RoomStatus.Dirty
            }
        };

        _context.Rooms.AddRange(rooms);

        // Add a check-in for room 102
        var checkIn = new CheckIn
        {
            Id = 1,
            RoomId = 2,
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Pax = 1,
            Status = CheckInStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.CheckIns.Add(checkIn);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRooms()
    {
        // Act
        var result = await _roomService.GetAllAsync();

        // Assert
        result.Should().HaveCount(4);
        result.Should().AllSatisfy(r => r.RoomNumber.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnRoom()
    {
        // Act
        var result = await _roomService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.RoomNumber.Should().Be("101");
        result.RoomStatus.Should().Be(RoomStatus.Available);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _roomService.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*RoomNo*999*");
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldReturnOnlyAvailableRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(5);
        var checkOutDate = DateTime.Today.AddDays(7);

        // Act
        var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate);

        // Assert
        result.Should().HaveCount(1); // Only Room 101 (Available)
        result.Should().Contain(r => r.RoomNumber == "101");
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldExcludeOccupiedRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = DateTime.Today.AddDays(2);

        // Act
        var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate);

        // Assert
        result.Should().NotContain(r => r.RoomNumber == "102"); // Room 102 is occupied
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldExcludeMaintenanceRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today;
        var checkOutDate = DateTime.Today.AddDays(1);

        // Act
        var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate);

        // Assert
        result.Should().NotContain(r => r.RoomNumber == "103"); // Room 103 is in maintenance
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateRoom()
    {
        // Arrange
        var dto = new CreateRoomDto
        {
            RoomNumber = "105",
            RoomTypeId = 1
        };

        // Act
        var result = await _roomService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.RoomNumber.Should().Be("105");
        result.RoomStatus.Should().Be(RoomStatus.Available);

        var roomInDb = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "105");
        roomInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateRoomNumber_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateRoomDto
        {
            RoomNumber = "101", // Already exists
            RoomTypeId = 1
        };

        // Act
        Func<Task> act = async () => await _roomService.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*101*already exists*");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidRoomType_ShouldThrowNotFoundException()
    {
        // Arrange
        var dto = new CreateRoomDto
        {
            RoomNumber = "106",
            RoomTypeId = 999 // Non-existent
        };

        // Act
        Func<Task> act = async () => await _roomService.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateStatusAsync_WithValidData_ShouldUpdateRoomStatus()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today,
            RoomStatusToDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _roomService.UpdateStatusAsync(1, dto);

        // Assert
        result.RoomStatus.Should().Be(RoomStatus.Maintenance);
        result.RoomStatusFromDate.Should().Be(DateTime.Today);
        result.RoomStatusToDate.Should().Be(DateTime.Today.AddDays(3));
    }

    [Fact]
    public async Task UpdateStatusAsync_ToAvailable_ShouldClearDates()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Available
        };

        // Act
        var result = await _roomService.UpdateStatusAsync(2, dto);

        // Assert
        result.RoomStatus.Should().Be(RoomStatus.Available);
        result.RoomStatusFromDate.Should().BeNull();
        result.RoomStatusToDate.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteRoom()
    {
        // Act
        await _roomService.DeleteAsync(1);

        // Assert
        var roomInDb = await _context.Rooms.FindAsync(1);
        roomInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _roomService.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetRoomIdByNumberAsync_WithValidNumber_ShouldReturnId()
    {
        // Act
        var result = await _roomService.GetRoomIdByNumberAsync("101");

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task GetRoomIdByNumberAsync_WithInvalidNumber_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _roomService.GetRoomIdByNumberAsync("999");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
