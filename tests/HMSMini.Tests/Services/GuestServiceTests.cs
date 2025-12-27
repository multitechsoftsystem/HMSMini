using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Implementations;

namespace HMSMini.Tests.Services;

public class GuestServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GuestService _guestService;
    private readonly Mock<ILogger<GuestService>> _loggerMock;

    public GuestServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<GuestService>>();
        _guestService = new GuestService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var guests = new List<Guest>
        {
            new Guest
            {
                Id = 1,
                CheckInId = 1,
                GuestNumber = 1,
                GuestName = "John Doe",
                Address = "123 Main St",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                MobileNo = "9876543210",
                PanOrAadharNo = "ABCDE1234F",
                Photo1Path = null,
                Photo2Path = null
            },
            new Guest
            {
                Id = 2,
                CheckInId = 1,
                GuestNumber = 2,
                GuestName = "Jane Doe",
                Address = "123 Main St",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                MobileNo = "9876543211",
                PanOrAadharNo = "FGHIJ5678K"
            }
        };

        _context.Guests.AddRange(guests);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnGuest()
    {
        // Act
        var result = await _guestService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.GuestName.Should().Be("John Doe");
        result.GuestNumber.Should().Be(1);
        result.CheckInId.Should().Be(1);
        result.PanOrAadharNo.Should().Be("ABCDE1234F");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _guestService.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Guest*999*");
    }

    [Fact]
    public async Task GetByCheckInIdAsync_ShouldReturnAllGuestsForCheckIn()
    {
        // Act
        var result = await _guestService.GetByCheckInIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.CheckInId.Should().Be(1));
        result.Should().BeInAscendingOrder(g => g.GuestNumber);
    }

    [Fact]
    public async Task GetByCheckInIdAsync_WithNoGuests_ShouldReturnEmptyList()
    {
        // Act
        var result = await _guestService.GetByCheckInIdAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateGuest()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "Updated Name",
            Address = "456 New St",
            City = "Delhi",
            State = "Delhi",
            Country = "India",
            MobileNo = "1234567890",
            PanOrAadharNo = "NEWPAN1234A"
        };

        // Act
        var result = await _guestService.UpdateAsync(1, dto);

        // Assert
        result.GuestName.Should().Be("Updated Name");
        result.Address.Should().Be("456 New St");
        result.City.Should().Be("Delhi");
        result.State.Should().Be("Delhi");
        result.MobileNo.Should().Be("1234567890");
        result.PanOrAadharNo.Should().Be("NEWPAN1234A");

        // Verify in database
        var guestInDb = await _context.Guests.FindAsync(1);
        guestInDb.Should().NotBeNull();
        guestInDb!.GuestName.Should().Be("Updated Name");
        guestInDb.PanOrAadharNo.Should().Be("NEWPAN1234A");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "Test"
        };

        // Act
        Func<Task> act = async () => await _guestService.UpdateAsync(999, dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePhotoPathAsync_WithValidData_ShouldUpdatePhoto1Path()
    {
        // Arrange
        var photoPath = "wwwroot/uploads/idproofs/1/1_guest1_photo1_20251227.jpg";

        // Act
        var result = await _guestService.UpdatePhotoPathAsync(1, 1, photoPath);

        // Assert
        result.Photo1Path.Should().Be(photoPath);
        result.Photo2Path.Should().BeNull();

        // Verify in database
        var guestInDb = await _context.Guests.FindAsync(1);
        guestInDb!.Photo1Path.Should().Be(photoPath);
    }

    [Fact]
    public async Task UpdatePhotoPathAsync_WithPhotoNumber2_ShouldUpdatePhoto2Path()
    {
        // Arrange
        var photoPath = "wwwroot/uploads/idproofs/1/1_guest1_photo2_20251227.jpg";

        // Act
        var result = await _guestService.UpdatePhotoPathAsync(1, 2, photoPath);

        // Assert
        result.Photo2Path.Should().Be(photoPath);
        result.Photo1Path.Should().BeNull();

        // Verify in database
        var guestInDb = await _context.Guests.FindAsync(1);
        guestInDb!.Photo2Path.Should().Be(photoPath);
    }

    [Fact]
    public async Task UpdatePhotoPathAsync_WithInvalidPhotoNumber_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var photoPath = "some/path.jpg";

        // Act
        Func<Task> act = async () => await _guestService.UpdatePhotoPathAsync(1, 3, photoPath);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Photo number must be 1 or 2*");
    }

    [Fact]
    public async Task UpdatePhotoPathAsync_WithInvalidGuestId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _guestService.UpdatePhotoPathAsync(999, 1, "path.jpg");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteGuest()
    {
        // Act
        await _guestService.DeleteAsync(1);

        // Assert
        var guestInDb = await _context.Guests.FindAsync(1);
        guestInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _guestService.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePhotoPathAsync_ShouldPreserveOtherGuestData()
    {
        // Arrange
        var photoPath = "new/photo.jpg";
        var originalGuest = await _guestService.GetByIdAsync(1);

        // Act
        await _guestService.UpdatePhotoPathAsync(1, 1, photoPath);
        var updatedGuest = await _guestService.GetByIdAsync(1);

        // Assert
        updatedGuest.GuestName.Should().Be(originalGuest.GuestName);
        updatedGuest.Address.Should().Be(originalGuest.Address);
        updatedGuest.City.Should().Be(originalGuest.City);
        updatedGuest.State.Should().Be(originalGuest.State);
        updatedGuest.Country.Should().Be(originalGuest.Country);
        updatedGuest.MobileNo.Should().Be(originalGuest.MobileNo);
        updatedGuest.Photo1Path.Should().Be(photoPath);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
