using HMSMini.Web.Models;
using System.Net.Http.Json;

namespace HMSMini.Web.Services;

public interface IApiClientService
{
    // Rooms
    Task<List<RoomDto>> GetRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<RoomDto?> CreateRoomAsync(CreateRoomDto room);

    // Reservations
    Task<List<ReservationDto>> GetReservationsAsync();
    Task<List<ReservationDto>> GetUpcomingReservationsAsync();
    Task<ReservationDto?> GetReservationByIdAsync(int id);
    Task<ReservationDto?> CreateReservationAsync(CreateReservationDto reservation);
    Task<ReservationDto?> ConfirmReservationAsync(int id);
    Task<bool> CancelReservationAsync(int id);

    // Check-Ins
    Task<List<CheckInDto>> GetCheckInsAsync();
    Task<List<CheckInDto>> GetActiveCheckInsAsync();
    Task<CheckInWithGuestsDto?> GetCheckInByIdAsync(int id);
    Task<CheckInWithGuestsDto?> CreateCheckInAsync(CreateCheckInDto checkIn);
    Task<bool> CheckOutAsync(int id);

    // Guests
    Task<GuestDto?> GetGuestByIdAsync(int id);
    Task<List<GuestDto>> GetGuestsByCheckInIdAsync(int checkInId);
    Task<GuestDto?> UpdateGuestAsync(int id, CreateGuestDto guest);

    // Room Types
    Task<List<RoomTypeDto>> GetRoomTypesAsync();
}

public class ApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;

    public ApiClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Rooms
    public async Task<List<RoomDto>> GetRoomsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoomDto>>("/api/rooms") ?? new();
        }
        catch { return new(); }
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RoomDto>($"/api/rooms/{id}");
        }
        catch { return null; }
    }

    public async Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        try
        {
            var checkInStr = checkIn.ToString("yyyy-MM-dd");
            var checkOutStr = checkOut.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<RoomDto>>(
                $"/api/rooms/available?checkIn={checkInStr}&checkOut={checkOutStr}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<RoomDto?> CreateRoomAsync(CreateRoomDto room)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/rooms", room);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<RoomDto>()
                : null;
        }
        catch { return null; }
    }

    // Reservations
    public async Task<List<ReservationDto>> GetReservationsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>("/api/reservations") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<ReservationDto>> GetUpcomingReservationsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>("/api/reservations/upcoming") ?? new();
        }
        catch { return new(); }
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ReservationDto>($"/api/reservations/{id}");
        }
        catch { return null; }
    }

    public async Task<ReservationDto?> CreateReservationAsync(CreateReservationDto reservation)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/reservations", reservation);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<ReservationDto?> ConfirmReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/reservations/{id}/confirm", null);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/reservations/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Check-Ins
    public async Task<List<CheckInDto>> GetCheckInsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CheckInDto>>("/api/checkins") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<CheckInDto>> GetActiveCheckInsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CheckInDto>>("/api/checkins/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<CheckInWithGuestsDto?> GetCheckInByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CheckInWithGuestsDto>($"/api/checkins/{id}");
        }
        catch { return null; }
    }

    public async Task<CheckInWithGuestsDto?> CreateCheckInAsync(CreateCheckInDto checkIn)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/checkins", checkIn);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> CheckOutAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/checkins/{id}/checkout", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Guests
    public async Task<GuestDto?> GetGuestByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GuestDto>($"/api/guests/{id}");
        }
        catch { return null; }
    }

    public async Task<List<GuestDto>> GetGuestsByCheckInIdAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestDto>>($"/api/guests/checkin/{checkInId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<GuestDto?> UpdateGuestAsync(int id, CreateGuestDto guest)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/guests/{id}", guest);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<GuestDto>()
                : null;
        }
        catch { return null; }
    }

    // Room Types
    public async Task<List<RoomTypeDto>> GetRoomTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoomTypeDto>>("/api/roomtypes") ?? new();
        }
        catch { return new(); }
    }
}
