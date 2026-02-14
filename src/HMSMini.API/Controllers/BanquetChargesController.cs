using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetCharge;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-charges")]
[Authorize]
public class BanquetChargesController : ControllerBase
{
    private readonly IBanquetChargeService _chargeService;
    private readonly ILogger<BanquetChargesController> _logger;

    public BanquetChargesController(IBanquetChargeService chargeService, ILogger<BanquetChargesController> logger)
    {
        _chargeService = chargeService;
        _logger = logger;
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<BanquetChargeDto>>> GetByBooking(int bookingId)
    {
        var charges = await _chargeService.GetByBookingAsync(bookingId);
        return Ok(charges);
    }

    [HttpPost("booking/{bookingId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetChargeDto>> Create(int bookingId, [FromBody] CreateBanquetChargeDto dto)
    {
        var charge = await _chargeService.CreateAsync(bookingId, dto);
        return Created($"api/banquet-charges/{charge.Id}", charge);
    }

    [HttpPut("{chargeId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetChargeDto>> Update(int chargeId, [FromBody] UpdateBanquetChargeDto dto)
    {
        var charge = await _chargeService.UpdateAsync(chargeId, dto);
        return Ok(charge);
    }

    [HttpDelete("{chargeId}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult> Delete(int chargeId)
    {
        await _chargeService.DeleteAsync(chargeId);
        return NoContent();
    }
}
