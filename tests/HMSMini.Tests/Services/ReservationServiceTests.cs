using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Reservation;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Implementations;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.Tests.Services;

public class ReservationServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReservationService _reservationService;
    private readonly Mock<IRoomService> _mockRoomService;
    private readonly Mock<ILogger<ReservationService>> _mockLogger;

    public ReservationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockRoomService = new Mock<IRoomService>();
        _mockLogger = new Mock<ILogger<ReservationService>>();
        _reservationService = new ReservationService(_context, _mockRoomService.Object, _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var roomType = new MRoomType { RoomTypeId = 1, RoomType = "Single", RoomDescription = "Single room" };
        var room = new RoomNo
        {
            RoomId = 1,
            RoomNumber = "101",
            RoomTypeId = 1,
            RoomType = roomType,
            RoomStatus = RoomStatus.Available
        };

        _context.RoomTypes.Add(roomType);
        _context.Rooms.Add(room);
        _context.SaveChanges();

        _mockRoomService.Setup(x => x.GetRoomIdByNumberAsync("101"))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateReservation()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 2,
            GuestName = "John Doe",
            GuestEmail = "john@example.com",
            GuestMobile = "1234567890",
            SpecialRequests = "Early check-in requested"
        };

        // Act
        var result = await _reservationService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().Be("John Doe");
        result.GuestEmail.Should().Be("john@example.com");
        result.GuestMobile.Should().Be("1234567890");
        result.NumberOfGuests.Should().Be(2);
        result.Status.Should().Be(ReservationStatus.Pending);
        result.ReservationNumber.Should().StartWith("RES");
        result.SpecialRequests.Should().Be("Early check-in requested");
    }

    [Fact]
    public async Task CreateAsync_WithCheckOutBeforeCheckIn_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(7),
            CheckOutDate = DateTime.Today.AddDays(5),
            NumberOfGuests = 1,
            GuestName = "John Doe",
            GuestMobile = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _reservationService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithPastCheckInDate_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(-5),
            CheckOutDate = DateTime.Today.AddDays(-3),
            NumberOfGuests = 1,
            GuestName = "John Doe",
            GuestMobile = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _reservationService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidNumberOfGuests_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 4, // More than 3
            GuestName = "John Doe",
            GuestMobile = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _reservationService.CreateAsync(dto));
    }

    [Fact]
    public async Task ConfirmReservationAsync_WithPendingReservation_ShouldConfirm()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280001",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Jane Doe",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.ConfirmReservationAsync(reservation.Id);

        // Assert
        result.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmReservationAsync_WithNonPendingReservation_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280002",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Jane Doe",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Confirmed, // Already confirmed
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _reservationService.ConfirmReservationAsync(reservation.Id));
    }

    [Fact]
    public async Task CancelReservationAsync_WithActiveReservation_ShouldCancel()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280003",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Jane Doe",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act
        await _reservationService.CancelReservationAsync(reservation.Id);

        // Assert
        var cancelled = await _context.Reservations.FindAsync(reservation.Id);
        cancelled!.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task CancelReservationAsync_WithCheckedInReservation_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280004",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Jane Doe",
            GuestMobile = "1234567890",
            Status = ReservationStatus.CheckedIn, // Already checked in
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _reservationService.CancelReservationAsync(reservation.Id));
    }

    [Fact]
    public async Task GetUpcomingReservationsAsync_ShouldReturnOnlyUpcomingReservations()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            new Reservation
            {
                ReservationNumber = "RES202512280005",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(7),
                NumberOfGuests = 1,
                GuestName = "Future Guest 1",
                GuestMobile = "1234567890",
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            },
            new Reservation
            {
                ReservationNumber = "RES202512280006",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(-5),
                CheckOutDate = DateTime.Today.AddDays(-3),
                NumberOfGuests = 1,
                GuestName = "Past Guest",
                GuestMobile = "1234567890",
                Status = ReservationStatus.CheckedIn, // Past reservation
                CreatedAt = DateTime.UtcNow
            },
            new Reservation
            {
                ReservationNumber = "RES202512280007",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(10),
                CheckOutDate = DateTime.Today.AddDays(12),
                NumberOfGuests = 1,
                GuestName = "Future Guest 2",
                GuestMobile = "1234567890",
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.GetUpcomingReservationsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.CheckInDate.Should().BeOnOrAfter(DateTime.Today));
        result.Should().AllSatisfy(r =>
            r.Status.Should().BeOneOf(ReservationStatus.Confirmed, ReservationStatus.Pending));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateReservation()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280008",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Original Name",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateReservationDto
        {
            GuestName = "Updated Name",
            NumberOfGuests = 2,
            SpecialRequests = "Late checkout please"
        };

        // Act
        var result = await _reservationService.UpdateAsync(reservation.Id, updateDto);

        // Assert
        result.GuestName.Should().Be("Updated Name");
        result.NumberOfGuests.Should().Be(2);
        result.SpecialRequests.Should().Be("Late checkout please");
    }

    [Fact]
    public async Task UpdateAsync_WithCheckedInReservation_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280009",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Guest Name",
            GuestMobile = "1234567890",
            Status = ReservationStatus.CheckedIn,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateReservationDto
        {
            GuestName = "Updated Name"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _reservationService.UpdateAsync(reservation.Id, updateDto));
    }

    [Fact]
    public async Task GetByReservationNumberAsync_WithValidNumber_ShouldReturnReservation()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280010",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.GetByReservationNumberAsync("RES202512280010");

        // Assert
        result.Should().NotBeNull();
        result.ReservationNumber.Should().Be("RES202512280010");
        result.GuestName.Should().Be("Test Guest");
    }

    [Fact]
    public async Task GetByReservationNumberAsync_WithInvalidNumber_ShouldThrowNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _reservationService.GetByReservationNumberAsync("INVALID123"));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteReservation()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationNumber = "RES202512280011",
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Delete Test",
            GuestMobile = "1234567890",
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act
        await _reservationService.DeleteAsync(reservation.Id);

        // Assert
        var deleted = await _context.Reservations.FindAsync(reservation.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnReservationsWithSpecifiedStatus()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            new Reservation
            {
                ReservationNumber = "RES202512280012",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(7),
                NumberOfGuests = 1,
                GuestName = "Confirmed Guest 1",
                GuestMobile = "1234567890",
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            },
            new Reservation
            {
                ReservationNumber = "RES202512280013",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(8),
                CheckOutDate = DateTime.Today.AddDays(10),
                NumberOfGuests = 1,
                GuestName = "Pending Guest",
                GuestMobile = "1234567890",
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new Reservation
            {
                ReservationNumber = "RES202512280014",
                RoomId = 1,
                CheckInDate = DateTime.Today.AddDays(11),
                CheckOutDate = DateTime.Today.AddDays(13),
                NumberOfGuests = 1,
                GuestName = "Confirmed Guest 2",
                GuestMobile = "1234567890",
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            }
        };
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.GetByStatusAsync(ReservationStatus.Confirmed);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.Status.Should().Be(ReservationStatus.Confirmed));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
