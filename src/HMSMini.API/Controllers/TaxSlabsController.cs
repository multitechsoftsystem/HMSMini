using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMSMini.API.Services.Interfaces;
using HMSMini.API.Models.DTOs.Tax;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaxSlabsController : ControllerBase
{
    private readonly ITaxService _taxService;
    private readonly ILogger<TaxSlabsController> _logger;

    public TaxSlabsController(ITaxService taxService, ILogger<TaxSlabsController> logger)
    {
        _taxService = taxService;
        _logger = logger;
    }

    /// <summary>
    /// Get all active tax slabs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TaxSlabDto>>> GetActiveTaxSlabs([FromQuery] DateTime? date = null)
    {
        try
        {
            var effectiveDate = date ?? DateTime.UtcNow;
            var taxSlabs = await _taxService.GetActiveTaxSlabsAsync(effectiveDate);
            return Ok(taxSlabs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tax slabs");
            return StatusCode(500, new List<TaxSlabDto>());
        }
    }

    /// <summary>
    /// Get applicable tax slab for a specific amount
    /// </summary>
    [HttpGet("applicable")]
    public async Task<ActionResult<ApplicableTaxSlabDto>> GetApplicableTaxSlab(
        [FromQuery] decimal amount,
        [FromQuery] DateTime? date = null)
    {
        try
        {
            if (amount < 0)
            {
                return BadRequest("Amount must be non-negative");
            }

            var effectiveDate = date ?? DateTime.UtcNow;
            var applicableSlab = await _taxService.GetApplicableTaxSlabAsync(amount, effectiveDate);

            if (applicableSlab == null)
            {
                return NotFound($"No applicable tax slab found for amount ₹{amount:N2}");
            }

            return Ok(applicableSlab);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applicable tax slab for amount {Amount}", amount);
            return StatusCode(500, "An error occurred while retrieving applicable tax slab");
        }
    }

    /// <summary>
    /// Calculate tax for a specific amount
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ActionResult<List<Models.DTOs.Billing.TaxLineDto>>> CalculateTax([FromBody] TaxCalculationRequest request)
    {
        try
        {
            var effectiveDate = request.Date ?? DateTime.UtcNow;
            var taxes = await _taxService.CalculateTaxAsync(
                request.Amount,
                request.TaxType,
                effectiveDate);

            return Ok(taxes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating tax for amount {Amount}", request.Amount);
            return StatusCode(500, "An error occurred while calculating tax");
        }
    }
}
