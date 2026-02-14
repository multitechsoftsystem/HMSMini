using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetBooking;
using HMSMini.API.Models.DTOs.BanquetBookingMenu;
using HMSMini.API.Models.DTOs.BanquetBookingService;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-bookings")]
[Authorize]
public class BanquetBookingsController : ControllerBase
{
    private readonly IBanquetBookingService _bookingService;
    private readonly ILogger<BanquetBookingsController> _logger;

    public BanquetBookingsController(IBanquetBookingService bookingService, ILogger<BanquetBookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<BanquetBookingListDto>>> GetAll()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BanquetBookingDetailDto>> GetById(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound($"Banquet booking with ID {id} not found.");
        return Ok(booking);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingDto>> Create([FromBody] CreateBanquetBookingDto dto)
    {
        var booking = await _bookingService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingDto>> Update(int id, [FromBody] UpdateBanquetBookingDto dto)
    {
        var booking = await _bookingService.UpdateAsync(id, dto);
        return Ok(booking);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _bookingService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingDto>> UpdateStatus(int id, [FromBody] UpdateBanquetBookingStatusDto dto)
    {
        var booking = await _bookingService.UpdateStatusAsync(id, dto);
        return Ok(booking);
    }

    // === Menus ===

    [HttpGet("{id}/menus")]
    public async Task<ActionResult<List<BanquetBookingMenuDto>>> GetMenus(int id)
    {
        var menus = await _bookingService.GetMenusByBookingAsync(id);
        return Ok(menus);
    }

    [HttpPost("{id}/menus")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingMenuDto>> AddMenu(int id, [FromBody] CreateBanquetBookingMenuDto dto)
    {
        var menu = await _bookingService.AddMenuAsync(id, dto);
        return CreatedAtAction(nameof(GetById), new { id }, menu);
    }

    [HttpPut("menus/{menuId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingMenuDto>> UpdateMenu(int menuId, [FromBody] UpdateBanquetBookingMenuDto dto)
    {
        var menu = await _bookingService.UpdateMenuAsync(menuId, dto);
        return Ok(menu);
    }

    [HttpDelete("menus/{menuId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult> DeleteMenu(int menuId)
    {
        await _bookingService.DeleteMenuAsync(menuId);
        return NoContent();
    }

    // === Services ===

    [HttpGet("{id}/services")]
    public async Task<ActionResult<List<BanquetBookingServiceDto>>> GetServices(int id)
    {
        var services = await _bookingService.GetServicesByBookingAsync(id);
        return Ok(services);
    }

    [HttpPost("{id}/services")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingServiceDto>> AddService(int id, [FromBody] CreateBanquetBookingServiceDto dto)
    {
        var service = await _bookingService.AddServiceAsync(id, dto);
        return CreatedAtAction(nameof(GetById), new { id }, service);
    }

    [HttpPut("services/{serviceId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetBookingServiceDto>> UpdateService(int serviceId, [FromBody] UpdateBanquetBookingServiceDto dto)
    {
        var service = await _bookingService.UpdateServiceAsync(serviceId, dto);
        return Ok(service);
    }

    [HttpDelete("services/{serviceId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult> DeleteService(int serviceId)
    {
        await _bookingService.DeleteServiceAsync(serviceId);
        return NoContent();
    }
}
