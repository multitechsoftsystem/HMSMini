using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Implementations;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.Tests.Services;

public class CheckInServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CheckInService _checkInService;
    private readonly Mock<IRoomService> _roomServiceMock;
    private readonly Mock<ILogger<CheckInService>> _loggerMock;

    public CheckInServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(options);
        _roomServiceMock = new Mock<IRoomService>();
        _loggerMock = new Mock<ILogger<CheckInService>>();
        _checkInService = new CheckInService(_context, _roomServiceMock.Object, _loggerMock.Object);

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

        var room = new RoomNo
        {
            RoomId = 1,
            RoomNumber = "101",
            RoomTypeId = 1,
            RoomStatus = RoomStatus.Available
        };

        _context.Rooms.Add(room);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateCheckInAsync_WithValidData_ShouldCreateCheckInAndGuests()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto
                {
                    GuestName = "John Doe",
                    Address = "123 Main St",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    MobileNo = "9876543210"
                }
            }
        };

        _roomServiceMock.Setup(x => x.GetRoomIdByNumberAsync("101"))
            .ReturnsAsync(1);

        _roomServiceMock.Setup(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RoomDto>
            {
                new RoomDto { RoomId = 1, RoomNumber = "101" }
            });

        // Act
        var result = await _checkInService.CreateCheckInAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.RoomNumber.Should().Be("101");
        result.Pax.Should().Be(1);
        result.Status.Should().Be(CheckInStatus.Active);
        result.Guests.Should().HaveCount(1);
        result.Guests.First().GuestName.Should().Be("John Doe");
        result.Guests.First().GuestNumber.Should().Be(1);

        // Verify room status was updated
        var room = await _context.Rooms.FindAsync(1);
        room.Should().NotBeNull();
        room!.RoomStatus.Should().Be(RoomStatus.Occupied);
    }

    [Fact]
    public async Task CreateCheckInAsync_WithMultipleGuests_ShouldCreateAllGuests()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1", MobileNo = "1111111111" },
                new CreateGuestDto { GuestName = "Guest 2", MobileNo = "2222222222" },
                new CreateGuestDto { GuestName = "Guest 3", MobileNo = "3333333333" }
            }
        };

        _roomServiceMock.Setup(x => x.GetRoomIdByNumberAsync("101")).ReturnsAsync(1);
        _roomServiceMock.Setup(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RoomDto> { new RoomDto { RoomId = 1 } });

        // Act
        var result = await _checkInService.CreateCheckInAsync(dto);

        // Assert
        result.Guests.Should().HaveCount(3);
        result.Guests.Should().Contain(g => g.GuestNumber == 1 && g.GuestName == "Guest 1");
        result.Guests.Should().Contain(g => g.GuestNumber == 2 && g.GuestName == "Guest 2");
        result.Guests.Should().Contain(g => g.GuestNumber == 3 && g.GuestName == "Guest 3");
        result.Pax.Should().Be(3);
    }

    [Fact]
    public async Task CreateCheckInAsync_WithInvalidDates_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(2),
            CheckOutDate = DateTime.Today, // Before check-in
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "John Doe" }
            }
        };

        // Act
        Func<Task> act = async () => await _checkInService.CreateCheckInAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Check-out date must be after check-in date*");
    }

    [Fact]
    public async Task CreateCheckInAsync_WithNoGuests_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>()
        };

        // Act
        Func<Task> act = async () => await _checkInService.CreateCheckInAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Number of guests must be between 1 and 3*");
    }

    [Fact]
    public async Task CreateCheckInAsync_WithTooManyGuests_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1" },
                new CreateGuestDto { GuestName = "Guest 2" },
                new CreateGuestDto { GuestName = "Guest 3" },
                new CreateGuestDto { GuestName = "Guest 4" }
            }
        };

        // Act
        Func<Task> act = async () => await _checkInService.CreateCheckInAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Number of guests must be between 1 and 3*");
    }

    [Fact]
    public async Task CreateCheckInAsync_WhenRoomNotAvailable_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "John Doe" }
            }
        };

        _roomServiceMock.Setup(x => x.GetRoomIdByNumberAsync("101")).ReturnsAsync(1);
        _roomServiceMock.Setup(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RoomDto>()); // No available rooms

        // Act
        Func<Task> act = async () => await _checkInService.CreateCheckInAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Room 101 is not available*");
    }

    [Fact]
    public async Task CheckOutAsync_WithActiveCheckIn_ShouldCheckOutAndSetRoomToDirty()
    {
        // Arrange
        var checkIn = new CheckIn
        {
            Id = 1,
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(-1),
            CheckOutDate = DateTime.Today.AddDays(1),
            Pax = 1,
            Status = CheckInStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.CheckIns.Add(checkIn);
        await _context.SaveChangesAsync();

        // Act
        await _checkInService.CheckOutAsync(1);

        // Assert
        var updatedCheckIn = await _context.CheckIns.Include(c => c.Room).FirstAsync(c => c.Id == 1);
        updatedCheckIn.Status.Should().Be(CheckInStatus.CheckedOut);
        updatedCheckIn.ActualCheckOutDate.Should().NotBeNull();
        updatedCheckIn.Room.RoomStatus.Should().Be(RoomStatus.Dirty);
        updatedCheckIn.Room.RoomStatusFromDate.Should().BeNull();
        updatedCheckIn.Room.RoomStatusToDate.Should().BeNull();
    }

    [Fact]
    public async Task CheckOutAsync_WithNonActiveCheckIn_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var checkIn = new CheckIn
        {
            Id = 1,
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(-2),
            CheckOutDate = DateTime.Today.AddDays(-1),
            Pax = 1,
            Status = CheckInStatus.CheckedOut,
            ActualCheckOutDate = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        _context.CheckIns.Add(checkIn);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _checkInService.CheckOutAsync(1);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Only active check-ins can be checked out*");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCheckInWithGuests()
    {
        // Arrange
        var checkIn = new CheckIn
        {
            Id = 1,
            RoomId = 1,
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Pax = 2,
            Status = CheckInStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Guests = new List<Guest>
            {
                new Guest { Id = 1, CheckInId = 1, GuestNumber = 1, GuestName = "Guest 1" },
                new Guest { Id = 2, CheckInId = 1, GuestNumber = 2, GuestName = "Guest 2" }
            }
        };
        _context.CheckIns.Add(checkIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _checkInService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Guests.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveCheckInsAsync_ShouldReturnOnlyActiveCheckIns()
    {
        // Arrange
        _context.CheckIns.AddRange(
            new CheckIn
            {
                Id = 1,
                RoomId = 1,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(2),
                Pax = 1,
                Status = CheckInStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new CheckIn
            {
                Id = 2,
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(-2),
                CheckOutDate = DateTime.Today.AddDays(-1),
                Pax = 1,
                Status = CheckInStatus.CheckedOut,
                ActualCheckOutDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _checkInService.GetActiveCheckInsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(CheckInStatus.Active);
    }

    [Fact]
    public async Task DeleteAsync_WithActiveCheckIn_ShouldFreeUpRoom()
    {
        // Arrange
        var checkIn = new CheckIn
        {
            Id = 1,
            RoomId = 1,
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Pax = 1,
            Status = CheckInStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.CheckIns.Add(checkIn);

        var room = await _context.Rooms.FindAsync(1);
        room!.RoomStatus = RoomStatus.Occupied;
        await _context.SaveChangesAsync();

        // Act
        await _checkInService.DeleteAsync(1);

        // Assert
        var deletedCheckIn = await _context.CheckIns.FindAsync(1);
        deletedCheckIn.Should().BeNull();

        var updatedRoom = await _context.Rooms.FindAsync(1);
        updatedRoom!.RoomStatus.Should().Be(RoomStatus.Available);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
