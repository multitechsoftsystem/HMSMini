using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.Reservation;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public class ReservationsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static int _roomCounter = 700; // Start from room 700 to avoid conflicts

    public ReservationsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(string RoomNumber, string Token)> CreateRoomAsync()
    {
        var managerToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);
        var roomNumber = Interlocked.Increment(ref _roomCounter).ToString();

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

        return (roomNumber, managerToken);
    }

    [Fact]
    public async Task CreateReservation_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        var dto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 2,
            GuestName = "John Doe",
            GuestEmail = "john@example.com",
            GuestMobile = "1234567890",
            SpecialRequests = "Early check-in"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(dto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        reservation.Should().NotBeNull();
        reservation!.GuestName.Should().Be("John Doe");
        reservation.GuestEmail.Should().Be("john@example.com");
        reservation.GuestMobile.Should().Be("1234567890");
        reservation.NumberOfGuests.Should().Be(2);
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.ReservationNumber.Should().NotBeNullOrEmpty();
        reservation.SpecialRequests.Should().Be("Early check-in");
    }

    [Fact]
    public async Task CreateReservation_WithInvalidDates_ShouldReturnBadRequest()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        var dto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(7),
            CheckOutDate = DateTime.Today.AddDays(5), // Before check-in
            NumberOfGuests = 1,
            GuestName = "John Doe",
            GuestMobile = "1234567890"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(dto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithPastCheckInDate_ShouldReturnBadRequest()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        var dto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(-5),
            CheckOutDate = DateTime.Today.AddDays(-3),
            NumberOfGuests = 1,
            GuestName = "John Doe",
            GuestMobile = "1234567890"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(dto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetReservationById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/reservations/{created!.Id}", token);

        // Act
        var response = await _client.SendAsync(getRequest);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        reservation.Should().NotBeNull();
        reservation!.Id.Should().Be(created.Id);
        reservation.GuestName.Should().Be("Test Guest");
    }

    [Fact]
    public async Task GetReservationById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Get, "/api/reservations/999999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReservationByNumber_WithValidNumber_ShouldReturnOk()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/reservations/number/{created!.ReservationNumber}", token);

        // Act
        var response = await _client.SendAsync(getRequest);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        reservation.Should().NotBeNull();
        reservation!.ReservationNumber.Should().Be(created.ReservationNumber);
    }

    [Fact]
    public async Task ConfirmReservation_WithPendingReservation_ShouldReturnOk()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        var confirmRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/reservations/{created!.Id}/confirm", token);

        // Act
        var response = await _client.SendAsync(confirmRequest);
        var confirmed = await response.Content.ReadFromJsonAsync<ReservationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        confirmed.Should().NotBeNull();
        confirmed!.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task CancelReservation_WithActiveReservation_ShouldReturnNoContent()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        var cancelRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/reservations/{created!.Id}/cancel", token);

        // Act
        var response = await _client.SendAsync(cancelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it was cancelled
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/reservations/{created.Id}", token);
        var getResponse = await _client.SendAsync(getRequest);
        var reservation = await getResponse.Content.ReadFromJsonAsync<ReservationDto>();
        reservation!.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task UpdateReservation_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Original Name",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        var updateDto = new UpdateReservationDto
        {
            GuestName = "Updated Name",
            NumberOfGuests = 2,
            SpecialRequests = "Late checkout"
        };

        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/reservations/{created!.Id}")
        {
            Content = JsonContent.Create(updateDto)
        };
        updateRequest.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(updateRequest);
        var updated = await response.Content.ReadFromJsonAsync<ReservationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updated.Should().NotBeNull();
        updated!.GuestName.Should().Be("Updated Name");
        updated.NumberOfGuests.Should().Be(2);
        updated.SpecialRequests.Should().Be("Late checkout");
    }

    [Fact]
    public async Task GetAllReservations_ShouldReturnOkWithList()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/reservations", token);

        // Act
        var response = await _client.SendAsync(request);
        var reservations = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        reservations.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUpcomingReservations_ShouldReturnOnlyUpcoming()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/reservations/upcoming", token);

        // Act
        var response = await _client.SendAsync(request);
        var reservations = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        reservations.Should().NotBeNull();
        reservations.Should().AllSatisfy(r => r.CheckInDate.Should().BeOnOrAfter(DateTime.Today));
    }

    [Fact]
    public async Task DeleteReservation_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var (roomNumber, _) = await CreateRoomAsync();
        var receptionistToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var adminToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);

        // Create a reservation first
        var createDto = new CreateReservationDto
        {
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Today.AddDays(5),
            CheckOutDate = DateTime.Today.AddDays(7),
            NumberOfGuests = 1,
            GuestName = "Test Guest",
            GuestMobile = "1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/reservations")
        {
            Content = JsonContent.Create(createDto)
        };
        createRequest.AddAuthorizationHeader(receptionistToken);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // Delete with admin token
        var deleteRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Delete, $"/api/reservations/{created!.Id}", adminToken);

        // Act
        var response = await _client.SendAsync(deleteRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/reservations/{created.Id}", adminToken);
        var getResponse = await _client.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteReservation_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var request = TestAuthHelper.CreateAuthenticatedRequest(
            HttpMethod.Delete, "/api/reservations/999999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
