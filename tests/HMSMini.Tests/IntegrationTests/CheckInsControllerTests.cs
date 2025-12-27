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
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/checkins", token);

        // Act
        var response = await _client.SendAsync(request);
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
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var actualCheckInDate = DateTime.UtcNow.AddMinutes(-30);
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            ActualCheckInDate = actualCheckInDate,
            RegistrationNo = "REG-INT-001",
            Remarks = "Integration test check-in",
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto
                {
                    GuestName = "Integration Test Guest",
                    Address = "123 Test Street",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    MobileNo = "9876543210",
                    PanOrAadharNo = "ABCDE1234F"
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
        var createdCheckIn = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCheckIn.Should().NotBeNull();
        createdCheckIn!.RoomNumber.Should().Be("101");
        createdCheckIn.Status.Should().Be(CheckInStatus.Active);
        createdCheckIn.ActualCheckInDate.Should().BeCloseTo(actualCheckInDate, TimeSpan.FromSeconds(1));
        createdCheckIn.RegistrationNo.Should().Be("REG-INT-001");
        createdCheckIn.Remarks.Should().Be("Integration test check-in");
        createdCheckIn.Pax.Should().Be(1);
        createdCheckIn.Guests.Should().HaveCount(1);
        createdCheckIn.Guests.First().GuestName.Should().Be("Integration Test Guest");
        createdCheckIn.Guests.First().PanOrAadharNo.Should().Be("ABCDE1234F");
    }

    [Fact]
    public async Task CreateCheckIn_WithMultipleGuests_ShouldCreateAllGuests()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "102",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(3),
            RegistrationNo = "REG-INT-002",
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1", MobileNo = "1111111111", PanOrAadharNo = "AAAAA1111A" },
                new CreateGuestDto { GuestName = "Guest 2", MobileNo = "2222222222", PanOrAadharNo = "BBBBB2222B" },
                new CreateGuestDto { GuestName = "Guest 3", MobileNo = "3333333333", PanOrAadharNo = "CCCCC3333C" }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);
        var createdCheckIn = await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCheckIn.Should().NotBeNull();
        createdCheckIn!.Pax.Should().Be(3);
        createdCheckIn.RegistrationNo.Should().Be("REG-INT-002");
        createdCheckIn.Guests.Should().HaveCount(3);
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 1 && g.GuestName == "Guest 1" && g.PanOrAadharNo == "AAAAA1111A");
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 2 && g.GuestName == "Guest 2" && g.PanOrAadharNo == "BBBBB2222B");
        createdCheckIn.Guests.Should().Contain(g => g.GuestNumber == 3 && g.GuestName == "Guest 3" && g.PanOrAadharNo == "CCCCC3333C");
    }

    [Fact]
    public async Task CreateCheckIn_WithInvalidDates_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCheckIn_WithNoGuests_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var checkInDto = new CreateCheckInDto
        {
            RoomNumber = "201",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>() // Empty guest list
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCheckIn_WithTooManyGuests_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
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

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        request.AddAuthorizationHeader(token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCheckInById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a check-in first
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
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/checkins/{createdCheckIn!.Id}", token);

        // Act
        var response = await _client.SendAsync(request);
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
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/checkins/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveCheckIns_ShouldReturnOnlyActiveCheckIns()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create an active check-in
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
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        createRequest.AddAuthorizationHeader(token);
        await _client.SendAsync(createRequest);

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/api/checkins/active", token);

        // Act
        var response = await _client.SendAsync(request);
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
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);

        // Create a check-in first
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
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        createRequest.AddAuthorizationHeader(token);
        var createResponse = await _client.SendAsync(createRequest);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Post, $"/api/checkins/{createdCheckIn!.Id}/checkout", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify check-in is marked as checked out
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/checkins/{createdCheckIn.Id}", token);
        var getResponse = await _client.SendAsync(getRequest);
        var checkedOutCheckIn = await getResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();
        checkedOutCheckIn!.Status.Should().Be(CheckInStatus.CheckedOut);
        checkedOutCheckIn.ActualCheckOutDate.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Post, "/api/checkins/999/checkout", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCheckIn_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var receptionistToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Receptionist);
        var adminToken = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);

        // Create a check-in first
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
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/checkins")
        {
            Content = JsonContent.Create(checkInDto)
        };
        createRequest.AddAuthorizationHeader(receptionistToken);
        var createResponse = await _client.SendAsync(createRequest);
        var createdCheckIn = await createResponse.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();

        // Delete with admin token
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, $"/api/checkins/{createdCheckIn!.Id}", adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify check-in is deleted
        var getRequest = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Get, $"/api/checkins/{createdCheckIn.Id}", adminToken);
        var getResponse = await _client.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCheckIn_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await TestAuthHelper.GetAuthTokenAsync(_client, UserRole.Admin);
        var request = TestAuthHelper.CreateAuthenticatedRequest(HttpMethod.Delete, "/api/checkins/999", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
