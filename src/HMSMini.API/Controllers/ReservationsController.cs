using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Reservation;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing hotel reservations
/// </summary>
[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(
        IReservationService reservationService,
        ILogger<ReservationsController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all reservations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ReservationDto>>> GetAll()
    {
        var reservations = await _reservationService.GetAllAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        return Ok(reservation);
    }

    /// <summary>
    /// Get reservation by reservation number
    /// </summary>
    [HttpGet("number/{reservationNumber}")]
    public async Task<ActionResult<ReservationDto>> GetByReservationNumber(string reservationNumber)
    {
        var reservation = await _reservationService.GetByReservationNumberAsync(reservationNumber);
        return Ok(reservation);
    }

    /// <summary>
    /// Get reservations by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<ReservationDto>>> GetByStatus(ReservationStatus status)
    {
        var reservations = await _reservationService.GetByStatusAsync(status);
        return Ok(reservations);
    }

    /// <summary>
    /// Get upcoming reservations
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<List<ReservationDto>>> GetUpcoming()
    {
        var reservations = await _reservationService.GetUpcomingReservationsAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto dto)
    {
        var reservation = await _reservationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    /// <summary>
    /// Update a reservation
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ReservationDto>> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        var reservation = await _reservationService.UpdateAsync(id, dto);
        return Ok(reservation);
    }

    /// <summary>
    /// Confirm a reservation
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ReservationDto>> Confirm(int id)
    {
        var reservation = await _reservationService.ConfirmReservationAsync(id);
        return Ok(reservation);
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult> Cancel(int id)
    {
        await _reservationService.CancelReservationAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Delete a reservation
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _reservationService.DeleteAsync(id);
        return NoContent();
    }
}
