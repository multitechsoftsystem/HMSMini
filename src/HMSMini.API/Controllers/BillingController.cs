using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(IBillingService billingService, ILogger<BillingController> logger)
    {
        _billingService = billingService;
        _logger = logger;
    }

    /// <summary>
    /// Add additional charge to a check-in
    /// </summary>
    [HttpPost("checkin/{checkInId}/charges")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdditionalChargeDto>> AddCharge(
        int checkInId,
        [FromBody] CreateAdditionalChargeDto dto)
    {
        try
        {
            var username = User.Identity?.Name;
            var charge = await _billingService.AddChargeAsync(checkInId, dto, username);
            return CreatedAtAction(nameof(GetCharges), new { checkInId }, charge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding charge to check-in {CheckInId}", checkInId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get all additional charges for a check-in
    /// </summary>
    [HttpGet("checkin/{checkInId}/charges")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AdditionalChargeDto>>> GetCharges(int checkInId)
    {
        var charges = await _billingService.GetChargesByCheckInAsync(checkInId);
        return Ok(charges);
    }

    /// <summary>
    /// Update an additional charge
    /// </summary>
    [HttpPut("charges/{chargeId}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdditionalChargeDto>> UpdateCharge(
        int chargeId,
        [FromBody] UpdateAdditionalChargeDto dto)
    {
        try
        {
            var charge = await _billingService.UpdateChargeAsync(chargeId, dto);
            if (charge == null)
                return NotFound($"Charge with ID {chargeId} not found");

            return Ok(charge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating charge {ChargeId}", chargeId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete an additional charge
    /// </summary>
    [HttpDelete("charges/{chargeId}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCharge(int chargeId)
    {
        try
        {
            var username = User.Identity?.Name;
            var deleted = await _billingService.DeleteChargeAsync(chargeId, username);
            if (!deleted)
                return NotFound($"Charge with ID {chargeId} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting charge {ChargeId}", chargeId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get invoice by invoice ID
    /// </summary>
    [HttpGet("invoices/{invoiceId}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int invoiceId)
    {
        var invoice = await _billingService.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null)
            return NotFound($"Invoice with ID {invoiceId} not found");

        return Ok(invoice);
    }
}
