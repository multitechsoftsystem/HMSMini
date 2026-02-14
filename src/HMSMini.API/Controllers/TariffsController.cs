using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Tariff;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing tariffs and calculating pricing
/// </summary>
[ApiController]
[Route("api/tariffs")]
[Authorize]
public class TariffsController : ControllerBase
{
    private readonly ITariffService _tariffService;
    private readonly ILogger<TariffsController> _logger;

    public TariffsController(
        ITariffService tariffService,
        ILogger<TariffsController> logger)
    {
        _tariffService = tariffService;
        _logger = logger;
    }

    /// <summary>
    /// Calculate tariff for a specific room type and occupancy
    /// </summary>
    [HttpGet("calculate")]
    public async Task<ActionResult<TariffCalculationDto>> CalculateTariff(
        [FromQuery] int roomTypeId,
        [FromQuery] int occupancy,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int? companyId = null,
        [FromQuery] int? mealPlanId = null)
    {
        var calculation = await _tariffService.CalculateTariffAsync(
            roomTypeId, occupancy, checkIn, checkOut, companyId, mealPlanId);
        return Ok(calculation);
    }

    /// <summary>
    /// Calculate tariffs for all room types
    /// </summary>
    [HttpGet("calculate-all")]
    public async Task<ActionResult<List<TariffCalculationDto>>> CalculateAllRoomTypes(
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int? companyId = null)
    {
        var calculations = await _tariffService.CalculateAllRoomTypesAsync(
            checkIn, checkOut, companyId);
        return Ok(calculations);
    }

    #region Base Tariffs

    /// <summary>
    /// Get all base tariffs
    /// </summary>
    [HttpGet("base")]
    public async Task<ActionResult<List<BaseTariffDto>>> GetBaseTariffs()
    {
        var tariffs = await _tariffService.GetBaseTariffsAsync();
        return Ok(tariffs);
    }

    /// <summary>
    /// Get base tariff by ID
    /// </summary>
    [HttpGet("base/{id}")]
    public async Task<ActionResult<BaseTariffDto>> GetBaseTariffById(int id)
    {
        var tariff = await _tariffService.GetBaseTariffByIdAsync(id);
        if (tariff == null)
        {
            return NotFound($"Base tariff with ID {id} not found.");
        }
        return Ok(tariff);
    }

    /// <summary>
    /// Get base tariffs by room type
    /// </summary>
    [HttpGet("base/room-type/{roomTypeId}")]
    public async Task<ActionResult<List<BaseTariffDto>>> GetBaseTariffsByRoomType(int roomTypeId)
    {
        var tariffs = await _tariffService.GetBaseTariffsByRoomTypeAsync(roomTypeId);
        return Ok(tariffs);
    }

    /// <summary>
    /// Create a new base tariff
    /// </summary>
    [HttpPost("base")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BaseTariffDto>> CreateBaseTariff([FromBody] CreateBaseTariffDto dto)
    {
        var tariff = await _tariffService.CreateBaseTariffAsync(dto);
        return CreatedAtAction(nameof(GetBaseTariffById), new { id = tariff.Id }, tariff);
    }

    /// <summary>
    /// Update a base tariff
    /// </summary>
    [HttpPut("base/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BaseTariffDto>> UpdateBaseTariff(int id, [FromBody] UpdateBaseTariffDto dto)
    {
        var tariff = await _tariffService.UpdateBaseTariffAsync(id, dto);
        return Ok(tariff);
    }

    /// <summary>
    /// Delete a base tariff
    /// </summary>
    [HttpDelete("base/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBaseTariff(int id)
    {
        await _tariffService.DeleteBaseTariffAsync(id);
        return NoContent();
    }

    #endregion

    #region Company Tariffs

    /// <summary>
    /// Get all tariffs for a specific company
    /// </summary>
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<List<CompanyTariffDto>>> GetCompanyTariffs(int companyId)
    {
        var tariffs = await _tariffService.GetCompanyTariffsAsync(companyId);
        return Ok(tariffs);
    }

    /// <summary>
    /// Get company tariff by ID
    /// </summary>
    [HttpGet("company/details/{id}")]
    public async Task<ActionResult<CompanyTariffDto>> GetCompanyTariffById(int id)
    {
        var tariff = await _tariffService.GetCompanyTariffByIdAsync(id);
        if (tariff == null)
        {
            return NotFound($"Company tariff with ID {id} not found.");
        }
        return Ok(tariff);
    }

    /// <summary>
    /// Create a new company tariff
    /// </summary>
    [HttpPost("company")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyTariffDto>> CreateCompanyTariff([FromBody] CreateCompanyTariffDto dto)
    {
        var tariff = await _tariffService.CreateCompanyTariffAsync(dto);
        return CreatedAtAction(nameof(GetCompanyTariffById), new { id = tariff.Id }, tariff);
    }

    /// <summary>
    /// Create multiple company tariffs at once
    /// </summary>
    [HttpPost("company/bulk")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<CompanyTariffDto>>> CreateBulkCompanyTariffs(
        [FromQuery] int companyId,
        [FromBody] List<CreateCompanyTariffDto> dtos)
    {
        var tariffs = await _tariffService.CreateBulkCompanyTariffsAsync(companyId, dtos);
        return Ok(tariffs);
    }

    /// <summary>
    /// Update a company tariff
    /// </summary>
    [HttpPut("company/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyTariffDto>> UpdateCompanyTariff(int id, [FromBody] UpdateCompanyTariffDto dto)
    {
        var tariff = await _tariffService.UpdateCompanyTariffAsync(id, dto);
        return Ok(tariff);
    }

    /// <summary>
    /// Delete a company tariff
    /// </summary>
    [HttpDelete("company/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCompanyTariff(int id)
    {
        await _tariffService.DeleteCompanyTariffAsync(id);
        return NoContent();
    }

    #endregion
}
