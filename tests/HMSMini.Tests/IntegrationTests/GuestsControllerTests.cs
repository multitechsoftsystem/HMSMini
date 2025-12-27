using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.DTOs.Room;

namespace HMSMini.Tests.IntegrationTests;

public class GuestsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static int _roomCounter = 600; // Start from room 600 to avoid conflicts with seed data

    public GuestsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<CheckInWithGuestsDto> CreateTestCheckInWithGuest()
    {
        var roomNumber = Interlocked.Increment(ref _roomCounter).ToString();

        // Create a room first
        var createRoomDto = new CreateRoomDto
        {
            RoomNumber = roomNumber,
            RoomTypeId = 1 // Single room type
        };
        await _client.PostAsJsonAsync("/api/rooms", createRoomDto);

        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto
                {
                    GuestName = "Test Guest for Guest Operations",
                    Address = "123 Test St",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    MobileNo = "9876543210",
                    PanOrAadharNo = "TESTPAN123X"
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Verify we got guests back
        if (result == null || result.Guests == null || !result.Guests.Any())
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create check-in with guest. Response: {errorContent}");
        }

        return result;
    }

    [Fact]
    public async Task GetGuestById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var checkIn = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/guests/{guestId}");
        var guest = await response.Content.ReadFromJsonAsync<GuestDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        guest.Should().NotBeNull();
        guest!.Id.Should().Be(guestId);
        guest.GuestName.Should().Be("Test Guest for Guest Operations");
    }

    [Fact]
    public async Task GetGuestById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/guests/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGuestsByCheckInId_ShouldReturnAllGuests()
    {
        // Arrange - Create check-in with multiple guests
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "102",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1" },
                new CreateGuestDto { GuestName = "Guest 2" }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/checkins", checkInDto);
        var checkIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Act
        var response = await _client.GetAsync($"/api/guests/checkin/{checkIn!.Id}");
        var guests = await response.Content.ReadFromJsonAsync<List<GuestDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        guests.Should().NotBeNull();
        guests.Should().HaveCount(2);
        guests.Should().BeInAscendingOrder(g => g.GuestNumber);
    }

    [Fact]
    public async Task UpdateGuest_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var checkIn = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        var updateDto = new CreateGuestDto
        {
            GuestName = "Updated Guest Name",
            Address = "456 New Address",
            City = "Delhi",
            State = "Delhi",
            Country = "India",
            MobileNo = "1234567890",
            PanOrAadharNo = "UPDATED1234Y"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/guests/{guestId}", updateDto);
        var updatedGuest = await response.Content.ReadFromJsonAsync<GuestDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedGuest.Should().NotBeNull();
        updatedGuest!.GuestName.Should().Be("Updated Guest Name");
        updatedGuest.City.Should().Be("Delhi");
        updatedGuest.MobileNo.Should().Be("1234567890");
        updatedGuest.PanOrAadharNo.Should().Be("UPDATED1234Y");
    }

    [Fact]
    public async Task UpdateGuest_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new CreateGuestDto
        {
            GuestName = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/guests/999", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGuest_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var checkIn = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        // Act
        var response = await _client.DeleteAsync($"/api/guests/{guestId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify guest is deleted
        var getResponse = await _client.GetAsync($"/api/guests/{guestId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGuest_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/guests/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadPhoto_WithMissingFile_ShouldReturnBadRequest()
    {
        // Arrange
        var checkIn = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("1"), "photoNumber");
        // No file added

        // Act
        var response = await _client.PostAsync($"/api/guests/{guestId}/upload-photo", content);

        // Assert
        // Should fail because no file provided
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task GetPhoto_WithInvalidGuestId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/guests/999/photos/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ProcessOcr_WithoutUploadedPhoto_ShouldReturnBadRequest()
    {
        // Arrange
        var checkIn = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        // Act - Try to process OCR without uploading a photo first
        var response = await _client.PostAsync($"/api/guests/{guestId}/process-ocr?photoNumber=1", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
