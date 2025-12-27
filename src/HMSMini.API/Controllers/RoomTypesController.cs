using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.RoomType;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages room types
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;
    private readonly ILogger<RoomTypesController> _logger;

    public RoomTypesController(IRoomTypeService roomTypeService, ILogger<RoomTypesController> logger)
    {
        _roomTypeService = roomTypeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all room types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoomTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomTypeDto>>> GetAll()
    {
        var roomTypes = await _roomTypeService.GetAllAsync();
        return Ok(roomTypes);
    }

    /// <summary>
    /// Get room type by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoomTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomTypeDto>> GetById(int id)
    {
        var roomType = await _roomTypeService.GetByIdAsync(id);
        return Ok(roomType);
    }

    /// <summary>
    /// Create a new room type
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomTypeDto>> Create([FromBody] CreateRoomTypeDto dto)
    {
        var roomType = await _roomTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = roomType.RoomTypeId }, roomType);
    }

    /// <summary>
    /// Update a room type
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomTypeDto>> Update(int id, [FromBody] UpdateRoomTypeDto dto)
    {
        var roomType = await _roomTypeService.UpdateAsync(id, dto);
        return Ok(roomType);
    }

    /// <summary>
    /// Delete a room type
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomTypeService.DeleteAsync(id);
        return NoContent();
    }
}
