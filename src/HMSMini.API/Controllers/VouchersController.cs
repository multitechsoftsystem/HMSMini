using HMSMini.API.Models.DTOs.Voucher;
using HMSMini.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for voucher operations (accounting ledger)
/// </summary>
[ApiController]
[Route("api/vouchers")]
[Authorize]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;
    private readonly ILogger<VouchersController> _logger;

    public VouchersController(
        IVoucherService voucherService,
        ILogger<VouchersController> logger)
    {
        _voucherService = voucherService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a voucher by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VoucherDto>> GetById(int id)
    {
        var voucher = await _voucherService.GetByIdAsync(id);

        if (voucher == null)
        {
            return NotFound($"Voucher with ID {id} not found.");
        }

        return Ok(voucher);
    }

    /// <summary>
    /// Gets all vouchers for a check-in
    /// </summary>
    [HttpGet("check-in/{checkInId}")]
    [ProducesResponseType(typeof(List<VoucherDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VoucherDto>>> GetByCheckInId(int checkInId)
    {
        var vouchers = await _voucherService.GetByCheckInIdAsync(checkInId);
        return Ok(vouchers);
    }

    /// <summary>
    /// Gets vouchers by date range
    /// </summary>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(List<VoucherDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<VoucherDto>>> GetByDateRange(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        if (fromDate > toDate)
        {
            return BadRequest("From date must be before or equal to to date.");
        }

        if ((toDate - fromDate).Days > 365)
        {
            return BadRequest("Date range cannot exceed 365 days.");
        }

        var vouchers = await _voucherService.GetByDateRangeAsync(fromDate, toDate);
        return Ok(vouchers);
    }

    /// <summary>
    /// Gets voucher summary by type for a date range
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(List<VoucherSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<VoucherSummaryDto>>> GetSummary(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        if (fromDate > toDate)
        {
            return BadRequest("From date must be before or equal to to date.");
        }

        if ((toDate - fromDate).Days > 365)
        {
            return BadRequest("Date range cannot exceed 365 days.");
        }

        var summary = await _voucherService.GetSummaryByDateRangeAsync(fromDate, toDate);
        return Ok(summary);
    }

    /// <summary>
    /// Creates a manual voucher (Admin/Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VoucherDto>> CreateVoucher([FromBody] CreateVoucherDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            dto.PostedBy = User.Identity?.Name ?? "System";
            var voucher = await _voucherService.CreateVoucherAsync(dto);

            _logger.LogInformation(
                "Manual voucher {VoucherNumber} created for CheckIn {CheckInId} by {User}",
                voucher.VoucherNumber,
                dto.CheckInId,
                dto.PostedBy);

            return CreatedAtAction(
                nameof(GetById),
                new { id = voucher.Id },
                voucher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating manual voucher for CheckIn {CheckInId}", dto.CheckInId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "An error occurred while creating the voucher.");
        }
    }

    /// <summary>
    /// Cancels a voucher (Admin/Manager only)
    /// Creates a reversing entry
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(VoucherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VoucherDto>> CancelVoucher(
        int id,
        [FromBody] CancelVoucherDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            dto.CancelledBy = User.Identity?.Name ?? "System";
            var cancelledVoucher = await _voucherService.CancelVoucherAsync(id, dto);

            _logger.LogInformation(
                "Voucher {VoucherNumber} cancelled by {User}",
                cancelledVoucher.VoucherNumber,
                dto.CancelledBy);

            return Ok(cancelledVoucher);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling voucher {VoucherId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "An error occurred while cancelling the voucher.");
        }
    }
}
