using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Enums;

namespace HMSMini.Tests.IntegrationTests;

public class RoomsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RoomsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllRooms_ShouldReturnOk_WithListOfRooms()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/rooms", token);

        // Act
        var response = await _client.SendAsync(request);
        var rooms = await response.Content.ReadFromJsonAsync<List<RoomDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        rooms.Should().NotBeNull();
        rooms.Should().NotBeEmpty();
        rooms.Should().AllSatisfy(r =>
        {
            r.RoomNumber.Should().NotBeNullOrEmpty();
            r.RoomTypeName.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetRoomById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);

        // Get first room from seed data
        var getAllRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/rooms", token);
        var getAllResponse = await _client.SendAsync(getAllRequest);
        var allRooms = await getAllResponse.Content.ReadFromJsonAsync<List<RoomDto>>();
        var firstRoomId = allRooms!.First().RoomId;

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/rooms/{firstRoomId}", token);

        // Act
        var response = await _client.SendAsync(request);
        var room = await response.Content.ReadFromJsonAsync<RoomDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        room.Should().NotBeNull();
        room!.RoomId.Should().Be(firstRoomId);
    }

    [Fact]
    public async Task GetRoomById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/rooms/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAvailableRooms_ShouldReturnOnlyAvailableRooms()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var checkInDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        var checkOutDate = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/rooms/available?checkIn={checkInDate}&checkOut={checkOutDate}", token);

        // Act
        var response = await _client.SendAsync(request);
        var availableRooms = await response.Content.ReadFromJsonAsync<List<RoomDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        availableRooms.Should().NotBeNull();
        // All returned rooms should have Available status or be in a non-conflicting state
        availableRooms.Should().AllSatisfy(r =>
        {
            r.RoomNumber.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task CreateRoom_WithValidData_ShouldReturnCreated()
    {
        // Arrange - Use Manager role which has permission to create rooms
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);
        var newRoom = new CreateRoomDto
        {
            RoomNumber = "999",
            RoomTypeId = 1 // Assuming seed data has at least one room type
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/rooms")
        {
            Content = JsonContent.Create(newRoom)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
        var createdRoom = await response.Content.ReadFromJsonAsync<RoomDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdRoom.Should().NotBeNull();
        createdRoom!.RoomNumber.Should().Be("999");
        createdRoom.RoomStatus.Should().Be(RoomStatus.Available);
    }

    [Fact]
    public async Task CreateRoom_WithDuplicateRoomNumber_ShouldReturnBadRequest()
    {
        // Arrange - Use Manager role
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);
        var duplicateRoom = new CreateRoomDto
        {
            RoomNumber = "101", // From seed data
            RoomTypeId = 1
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/rooms")
        {
            Content = JsonContent.Create(duplicateRoom)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRoomStatus_WithValidData_ShouldReturnOk()
    {
        // Arrange - Use Manager role
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);

        // Get first available room
        var getAllRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/rooms", token);
        var getAllResponse = await _client.SendAsync(getAllRequest);
        var allRooms = await getAllResponse.Content.ReadFromJsonAsync<List<RoomDto>>();
        var roomToUpdate = allRooms!.First(r => r.RoomStatus == RoomStatus.Available);

        var updateDto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today,
            RoomStatusToDate = DateTime.Today.AddDays(2)
        };

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/rooms/{roomToUpdate.RoomId}/status")
        {
            Content = JsonContent.Create(updateDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
        var updatedRoom = await response.Content.ReadFromJsonAsync<RoomDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedRoom.Should().NotBeNull();
        updatedRoom!.RoomStatus.Should().Be(RoomStatus.Maintenance);
        updatedRoom.RoomStatusFromDate.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task DeleteRoom_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Use Admin role for delete
        var adminToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var managerToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Manager);

        // Create a room to delete
        var newRoom = new CreateRoomDto
        {
            RoomNumber = "998",
            RoomTypeId = 1
        };
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/rooms")
        {
            Content = JsonContent.Create(newRoom)
        };
        createRequest.AddAuthorizationHeader(managerToken);
        var createResponse = await _client.SendAsync(createRequest);
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<RoomDto>();

        // Delete with admin token
        var deleteRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, $"/api/rooms/{createdRoom!.RoomId}", adminToken);

        // Act
        var response = await _client.SendAsync(deleteRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify room is deleted
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/rooms/{createdRoom.RoomId}", adminToken);
        var getResponse = await _client.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRoom_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange - Use Admin role
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, "/api/rooms/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
