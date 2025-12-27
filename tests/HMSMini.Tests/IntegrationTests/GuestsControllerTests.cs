using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public class GuestsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static int _roomCounter = 600; // Start from room 600 to avoid conflicts with seed data

    public GuestsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(CheckInWithGuestsDto CheckIn, string Token)> CreateTestCheckInWithGuest()
    {
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var managerToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);
        var roomNumber = Interlocked.Increment(ref _roomCounter).ToString();

        // Create a room first (requires Manager or Admin)
        var createRoomDto = new CreateRoomDto
        {
            RoomNumber = roomNumber,
            RoomTypeId = 1 // Single room type
        };
        var createRoomRequest = new HttpRequestMessage(HttpMethod.Post, "/api/rooms")
        {
            Content = JsonContent.Create(createRoomDto)
        };
        createRoomRequest.AddAuthorizationHeader(managerToken);
        await _client.SendAsync(createRoomRequest);

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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Verify we got guests back
        if (result == null || result.Guests == null || !result.Guests.Any())
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create check-in with guest. Response: {errorContent}");
        }

        return (result, token);
    }

    [Fact]
    public async Task GetGuestById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var (checkIn, token) = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/guests/{guestId}", token);

        // Act
        var response = await _client.SendAsync(request);
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
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/guests/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGuestsByCheckInId_ShouldReturnAllGuests()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create check-in with multiple guests
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
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var checkIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/guests/checkin/{checkIn!.Id}", token);

        // Act
        var response = await _client.SendAsync(request);
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
        var (checkIn, token) = await CreateTestCheckInWithGuest();
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/guests/{guestId}")
        {
            Content = JsonContent.Create(updateDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
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
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var updateDto = new CreateGuestDto
        {
            GuestName = "Test"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/guests/999")
        {
            Content = JsonContent.Create(updateDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGuest_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var (checkIn, _) = await CreateTestCheckInWithGuest();
        var adminToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var guestId = checkIn.Guests.First().Id;

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, $"/api/guests/{guestId}", adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify guest is deleted
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/guests/{guestId}", adminToken);
        var getResponse = await _client.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGuest_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, "/api/guests/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadPhoto_WithMissingFile_ShouldReturnBadRequest()
    {
        // Arrange
        var (checkIn, token) = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("1"), "photoNumber");
        // No file added

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/guests/{guestId}/upload-photo")
        {
            Content = content
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // Should fail because no file provided
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task GetPhoto_WithInvalidGuestId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/guests/999/photos/1", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ProcessOcr_WithoutUploadedPhoto_ShouldReturnBadRequest()
    {
        // Arrange
        var (checkIn, token) = await CreateTestCheckInWithGuest();
        var guestId = checkIn.Guests.First().Id;

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Post, $"/api/guests/{guestId}/process-ocr?photoNumber=1", token);

        // Act - Try to process OCR without uploading a photo first
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
