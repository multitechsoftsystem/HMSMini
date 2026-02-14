using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages guest check-ins
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInsController : ControllerBase
{
    private readonly ICheckInService _checkInService;
    private readonly IBillingService _billingService;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<CheckInsController> _logger;

    public CheckInsController(
        ICheckInService checkInService,
        IBillingService billingService,
        ISystemSettingsService systemSettingsService,
        ILogger<CheckInsController> logger)
    {
        _checkInService = checkInService;
        _billingService = billingService;
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    /// <summary>
    /// Get all check-ins
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CheckInDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CheckInDto>>> GetAll()
    {
        var checkIns = await _checkInService.GetAllAsync();
        return Ok(checkIns);
    }

    /// <summary>
    /// Get active check-ins
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<CheckInDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CheckInDto>>> GetActive()
    {
        var checkIns = await _checkInService.GetActiveCheckInsAsync();
        return Ok(checkIns);
    }

    /// <summary>
    /// Get check-in by ID with guest details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CheckInWithGuestsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CheckInWithGuestsDto>> GetById(int id)
    {
        var checkIn = await _checkInService.GetByIdAsync(id);
        return Ok(checkIn);
    }

    /// <summary>
    /// Create a new check-in with guests
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(CheckInWithGuestsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CheckInWithGuestsDto>> Create([FromBody] CreateCheckInDto dto)
    {
        // Validate working date (closed date restriction)
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        var isAdmin = User.IsInRole("Admin");

        if (dto.CheckInDate.Date < workingDate && !isAdmin)
        {
            _logger.LogWarning(
                "User {User} attempted to create check-in on closed date {CheckInDate}. Current working date: {WorkingDate}",
                User.Identity?.Name,
                dto.CheckInDate,
                workingDate);

            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    error = "Cannot create check-in on closed date",
                    checkInDate = dto.CheckInDate.Date,
                    workingDate = workingDate.Date,
                    message = "Only administrators can create check-ins on closed dates. Please contact your administrator."
                });
        }

        var checkIn = await _checkInService.CreateCheckInAsync(dto);

        if (!isAdmin && dto.CheckInDate.Date < workingDate)
        {
            _logger.LogInformation(
                "Admin {User} created check-in on closed date {CheckInDate} (override)",
                User.Identity?.Name,
                dto.CheckInDate);
        }

        return CreatedAtAction(nameof(GetById), new { id = checkIn.Id }, checkIn);
    }

    /// <summary>
    /// Extend a guest's stay
    /// </summary>
    [HttpPost("{id}/extend")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(CheckInDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckInDto>> ExtendStay(int id, [FromBody] ExtendStayDto dto)
    {
        try
        {
            var updatedCheckIn = await _checkInService.ExtendStayAsync(id, dto);
            return Ok(updatedCheckIn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending stay for check-in {CheckInId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Check out a guest
    /// </summary>
    [HttpPost("{id}/checkout")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut(int id)
    {
        await _checkInService.CheckOutAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Update check-in (Company, Business Source, Meal Plan, Remarks)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckInDto>> UpdateCheckIn(int id, [FromBody] UpdateCheckInDto dto)
    {
        try
        {
            var result = await _checkInService.UpdateCheckInAsync(id, dto);
            if (result == null)
                return NotFound($"Check-in with ID {id} not found");

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get bill preview for a check-in before checkout
    /// </summary>
    [HttpGet("{id}/bill-preview")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BillPreviewDto>> GetBillPreview(
        int id,
        [FromQuery] DateTime? checkOutDate = null)
    {
        try
        {
            var billPreview = await _billingService.GetBillPreviewAsync(id, checkOutDate);
            return Ok(billPreview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bill preview for check-in {CheckInId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Finalize checkout and create invoice
    /// </summary>
    [HttpPost("{id}/finalize-checkout")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InvoiceDto>> FinalizeCheckout(
        int id,
        [FromBody] FinalizeInvoiceDto dto)
    {
        try
        {
            var invoice = await _billingService.FinalizeInvoiceAsync(id, dto);
            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing checkout for check-in {CheckInId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get invoice for a checked-out check-in
    /// </summary>
    [HttpGet("{id}/invoice")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var invoice = await _billingService.GetInvoiceByCheckInIdAsync(id);
        if (invoice == null)
            return NotFound($"No invoice found for check-in ID {id}");

        return Ok(invoice);
    }

    /// <summary>
    /// Delete a check-in
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _checkInService.DeleteAsync(id);
        return NoContent();
    }
}
