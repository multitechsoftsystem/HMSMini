using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public class CheckInsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CheckInsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCheckIns_ShouldReturnOk_WithListOfCheckIns()
    {
        // Act
        var response = await _client.GetAsync("/api/checkins");
        var checkIns = await response.Content.ReadFromJsonAsync<List<CheckInDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        checkIns.Should().NotBeNull();
        // May be empty if no check-ins exist yet
    }

    [Fact]
    public async Task CreateCheckIn_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto
                {
                    GuestName = "Integration Test Guest",
                    Address = "123 Test Street",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    MobileNo = "9876543210"
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var createdCheckIn = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCheckIn.Should().NotBeNull();
        createdCheckIn!.RoomNumber.Should().Be("101");
        createdCheckIn.Status.Should().Be(CheckInStatus.Active);
        createdCheckIn.Pax.Should().Be(1);
        createdCheckIn.Guests.Should().HaveCount(1);
        createdCheckIn.Guests.First().GuestName.Should().Be("Integration Test Guest");
    }

    [Fact]
    public async Task CreateCheckIn_WithMultipleGuests_ShouldCreateAllGuests()
    {
        // Arrange
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "102",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(3),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1", MobileNo = "1111111111" },
                new CreateGuestDto { GuestName = "Guest 2", MobileNo = "2222222222" },
                new CreateGuestDto { GuestName = "Guest 3", MobileNo = "3333333333" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var createdCheckIn = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCheckIn.Should().NotBeNull();
        createdCheckIn!.Pax.Should().Be(3);
        createdCheckIn.Guests.Should().HaveCount(3);
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 1 && g.GuestName == "Guest 1");
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 2 && g.GuestName == "Guest 2");
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 3 && g.GuestName == "Guest 3");
    }

    [Fact]
    public async Task CreateCheckIn_WithInvalidDates_ShouldReturnBadRequest()
    {
        // Arrange
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "103",
            CheckInDate = DateTime.Today.AddDays(2),
            CheckOutDate = DateTime.Today, // Before check-in date
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Test Guest" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCheckIn_WithNoGuests_ShouldReturnBadRequest()
    {
        // Arrange
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "201",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>() // Empty guest list
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCheckIn_WithTooManyGuests_ShouldReturnBadRequest()
    {
        // Arrange
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "302",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1" },
                new CreateGuestDto { GuestName = "Guest 2" },
                new CreateGuestDto { GuestName = "Guest 3" },
                new CreateGuestDto { GuestName = "Guest 4" } // 4 guests - too many
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCheckInById_WithValidId_ShouldReturnOk()
    {
        // Arrange - Create a check-in first
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "202",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Test Guest" }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Act
        var response = await _client.GetAsync($"/api/checkins/{createdCheckIn!.Id}");
        var checkIn = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        checkIn.Should().NotBeNull();
        checkIn!.Id.Should().Be(createdCheckIn.Id);
        checkIn.Guests.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCheckInById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/checkins/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveCheckIns_ShouldReturnOnlyActiveCheckIns()
    {
        // Arrange - Create an active check-in
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "203",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Active Guest" }
            }
        };
        await _client.PostAsJsonAsync("/api/checkins", checkInDto);

        // Act
        var response = await _client.GetAsync("/api/checkins/active");
        var activeCheckIns = await response.Content.ReadFromJsonAsync<List<CheckInDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        activeCheckIns.Should().NotBeNull();
        activeCheckIns.Should().NotBeEmpty();
        activeCheckIns.Should().AllSatisfy(c => c.Status.Should().Be(CheckInStatus.Active));
    }

    [Fact]
    public async Task CheckOut_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Create a check-in first
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "204",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Checkout Test Guest" }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Act
        var response = await _client.PostAsync($"/api/checkins/{createdCheckIn!.Id}/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify check-in is marked as checked out
        var getResponse = await _client.GetAsync($"/api/checkins/{createdCheckIn.Id}");
        var checkedOutCheckIn = await getResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();
        checkedOutCheckIn!.Status.Should().Be(CheckInStatus.CheckedOut);
        checkedOutCheckIn.ActualCheckOutDate.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PostAsync("/api/checkins/999/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCheckIn_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Create a check-in first
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "301",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Delete Test Guest" }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/checkins/{createdCheckIn!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify check-in is deleted
        var getResponse = await _client.GetAsync($"/api/checkins/{createdCheckIn.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCheckIn_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/checkins/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
