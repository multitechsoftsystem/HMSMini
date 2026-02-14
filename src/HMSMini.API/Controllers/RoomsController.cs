using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages hotel rooms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    /// <summary>
    /// Get all rooms
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Get room by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        return Ok(room);
    }

    /// <summary>
    /// Get available rooms for date range
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(List<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomDto>>> GetAvailable([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut);
        return Ok(rooms);
    }

    /// <summary>
    /// Get room availability for date range with daily status
    /// </summary>
    [HttpGet("availability")]
    [ProducesResponseType(typeof(List<RoomAvailabilityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomAvailabilityDto>>> GetAvailability([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var availability = await _roomService.GetRoomAvailabilityAsync(startDate, endDate);
        return Ok(availability);
    }

    /// <summary>
    /// Get occupancy report for a specific date
    /// </summary>
    [HttpGet("occupancy-report")]
    [ProducesResponseType(typeof(OccupancyReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OccupancyReportDto>> GetOccupancyReport([FromQuery] DateTime? date)
    {
        var reportDate = date ?? DateTime.Today;
        var report = await _roomService.GetOccupancyReportAsync(reportDate);
        return Ok(report);
    }

    /// <summary>
    /// Export occupancy report to Excel
    /// </summary>
    [HttpGet("occupancy-report/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportOccupancyReportToExcel([FromQuery] DateTime? date)
    {
        var reportDate = date ?? DateTime.Today;
        var excelData = await _roomService.ExportOccupancyReportToExcelAsync(reportDate);

        var fileName = $"OccupancyReport_{reportDate:yyyy-MM-dd}.xlsx";
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// Create a new room
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        var room = await _roomService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = room.RoomId }, room);
    }

    /// <summary>
    /// Update room status (Admin/Manager only - for blocking, maintenance, etc.)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> UpdateStatus(int id, [FromBody] UpdateRoomStatusDto dto)
    {
        var room = await _roomService.UpdateStatusAsync(id, dto);
        return Ok(room);
    }

    /// <summary>
    /// Mark room as clean (Housekeeping only - can only set to Available or Dirty)
    /// </summary>
    [HttpPut("{id}/cleaning-status")]
    [Authorize(Roles = "Admin,Manager,Housekeeping")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> UpdateCleaningStatus(int id, [FromBody] UpdateRoomStatusDto dto)
    {
        // Housekeeping can only set rooms to Available (0) or Dirty (2)
        if (dto.RoomStatus != Models.Enums.RoomStatus.Available &&
            dto.RoomStatus != Models.Enums.RoomStatus.Dirty)
        {
            return BadRequest("Housekeeping can only mark rooms as Available or Dirty");
        }

        // Clear any date ranges when setting cleaning status
        dto.RoomStatusFromDate = null;
        dto.RoomStatusToDate = null;

        var room = await _roomService.UpdateStatusAsync(id, dto);
        return Ok(room);
    }

    /// <summary>
    /// Set room maintenance status (Maintenance staff can set/release maintenance)
    /// </summary>
    [HttpPut("{id}/maintenance-status")]
    [Authorize(Roles = "Admin,Manager,Maintenance")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> UpdateMaintenanceStatus(int id, [FromBody] UpdateRoomStatusDto dto)
    {
        // Maintenance can only set rooms to Maintenance (3) or Available (0)
        if (dto.RoomStatus != Models.Enums.RoomStatus.Maintenance &&
            dto.RoomStatus != Models.Enums.RoomStatus.Available)
        {
            return BadRequest("Maintenance can only mark rooms as Maintenance or Available");
        }

        var room = await _roomService.UpdateStatusAsync(id, dto);
        return Ok(room);
    }

    /// <summary>
    /// Delete a room
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomService.DeleteAsync(id);
        return NoContent();
    }
}
