using HMSMini.API.Models.DTOs.DayClosing;
using HMSMini.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for daily closing operations
/// Only accessible by Admin and Manager roles
/// </summary>
[ApiController]
[Route("api/day-closing")]
[Authorize(Roles = "Admin,Manager")]
public class DailyCloseController : ControllerBase
{
    private readonly IDayClosingService _dayClosingService;
    private readonly ILogger<DailyCloseController> _logger;

    public DailyCloseController(
        IDayClosingService dayClosingService,
        ILogger<DailyCloseController> logger)
    {
        _dayClosingService = dayClosingService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current working date information
    /// </summary>
    [HttpGet("working-date")]
    [ProducesResponseType(typeof(WorkingDateDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkingDateDto>> GetWorkingDate()
    {
        var workingDate = await _dayClosingService.GetWorkingDateInfoAsync();
        return Ok(workingDate);
    }

    /// <summary>
    /// Validates if the current day can be closed
    /// </summary>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(DayCloseValidationDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DayCloseValidationDto>> ValidateDayClose()
    {
        var validation = await _dayClosingService.ValidateDayCloseAsync();
        return Ok(validation);
    }

    /// <summary>
    /// Gets a preview of vouchers that will be posted during day close
    /// </summary>
    [HttpGet("preview")]
    [ProducesResponseType(typeof(DayClosePreviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DayClosePreviewDto>> GetDayClosePreview()
    {
        var preview = await _dayClosingService.GetDayClosePreviewAsync();
        return Ok(preview);
    }

    /// <summary>
    /// Executes the day close operation
    /// </summary>
    [HttpPost("close")]
    [ProducesResponseType(typeof(DayCloseResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DayCloseResultDto>> CloseDay()
    {
        try
        {
            var username = User.Identity?.Name ?? "System";

            _logger.LogInformation(
                "Day close initiated by user: {Username}",
                username);

            var result = await _dayClosingService.CloseDayAsync(username);

            if (!result.Success)
            {
                _logger.LogError(
                    "Day close failed: {ErrorMessage}",
                    result.ErrorMessage);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    result);
            }

            _logger.LogInformation(
                "Day close completed successfully. Closed: {ClosedDate}, New Working Date: {NewDate}, " +
                "CheckIns: {CheckIns}, Vouchers: {Vouchers}, Revenue: {Revenue:C}",
                result.ClosedDate,
                result.NewWorkingDate,
                result.TotalActiveCheckIns,
                result.TotalVouchersPosted,
                result.TotalRevenuePosted);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during day close");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new DayCloseResultDto
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred during day close. Please contact support."
                });
        }
    }

    /// <summary>
    /// Gets day closing history
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<DayClosingAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DayClosingAuditDto>>> GetClosingHistory(
        [FromQuery] int pageSize = 30,
        [FromQuery] int skip = 0)
    {
        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Page size must be between 1 and 100.");
        }

        if (skip < 0)
        {
            return BadRequest("Skip must be non-negative.");
        }

        var history = await _dayClosingService.GetClosingHistoryAsync(pageSize, skip);
        return Ok(history);
    }
}
