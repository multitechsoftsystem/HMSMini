using HMSMini.API.Models.DTOs.Reservation;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for reservation operations
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// Get all reservations
    /// </summary>
    Task<List<ReservationDto>> GetAllAsync();

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    Task<ReservationDto> GetByIdAsync(int id);

    /// <summary>
    /// Get reservation by reservation number
    /// </summary>
    Task<ReservationDto> GetByReservationNumberAsync(string reservationNumber);

    /// <summary>
    /// Get reservations by status
    /// </summary>
    Task<List<ReservationDto>> GetByStatusAsync(ReservationStatus status);

    /// <summary>
    /// Get upcoming reservations (check-in date in the future and status is Confirmed or Pending)
    /// </summary>
    Task<List<ReservationDto>> GetUpcomingReservationsAsync();

    /// <summary>
    /// Create a new reservation
    /// </summary>
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);

    /// <summary>
    /// Update an existing reservation
    /// </summary>
    Task<ReservationDto> UpdateAsync(int id, UpdateReservationDto dto);

    /// <summary>
    /// Confirm a reservation
    /// </summary>
    Task<ReservationDto> ConfirmReservationAsync(int id);

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    Task CancelReservationAsync(int id);

    /// <summary>
    /// Mark reservation as checked in
    /// </summary>
    Task MarkAsCheckedInAsync(int reservationId, int checkInId);

    /// <summary>
    /// Delete a reservation
    /// </summary>
    Task DeleteAsync(int id);
}
