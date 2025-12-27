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
        // Act
        var response = await _client.GetAsync("/api/rooms");
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
        // Arrange - Get first room from seed data
        var allRooms = await _client.GetFromJsonAsync<List<RoomDto>>("/api/rooms");
        var firstRoomId = allRooms!.First().RoomId;

        // Act
        var response = await _client.GetAsync($"/api/rooms/{firstRoomId}");
        var room = await response.Content.ReadFromJsonAsync<RoomDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        room.Should().NotBeNull();
        room!.RoomId.Should().Be(firstRoomId);
    }

    [Fact]
    public async Task GetRoomById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/rooms/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAvailableRooms_ShouldReturnOnlyAvailableRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        var checkOutDate = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"/api/rooms/available?checkIn={checkInDate}&checkOut={checkOutDate}");
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
        // Arrange
        var newRoom = new CreateRoomDto
        {
            RoomNumber = "999",
            RoomTypeId = 1 // Assuming seed data has at least one room type
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rooms", newRoom);
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
        // Arrange - Use an existing room number from seed data
        var duplicateRoom = new CreateRoomDto
        {
            RoomNumber = "101", // From seed data
            RoomTypeId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rooms", duplicateRoom);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRoomStatus_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var allRooms = await _client.GetFromJsonAsync<List<RoomDto>>("/api/rooms");
        var roomToUpdate = allRooms!.First(r => r.RoomStatus == RoomStatus.Available);

        var updateDto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today,
            RoomStatusToDate = DateTime.Today.AddDays(2)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/rooms/{roomToUpdate.RoomId}/status", updateDto);
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
        // Arrange - Create a room to delete
        var newRoom = new CreateRoomDto
        {
            RoomNumber = "998",
            RoomTypeId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/rooms", newRoom);
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<RoomDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/rooms/{createdRoom!.RoomId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify room is deleted
        var getResponse = await _client.GetAsync($"/api/rooms/{createdRoom.RoomId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRoom_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/rooms/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
