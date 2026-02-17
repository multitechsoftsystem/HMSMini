using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.ChartOfAccount;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/chart-of-accounts")]
[Authorize(Roles = "Admin,Manager")]
public class ChartOfAccountsController : ControllerBase
{
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<ChartOfAccountsController> _logger;

    public ChartOfAccountsController(IChartOfAccountService chartOfAccountService, ILogger<ChartOfAccountsController> logger)
    {
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ChartOfAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ChartOfAccountDto>>> GetAll()
    {
        var result = await _chartOfAccountService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChartOfAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChartOfAccountDto>> GetById(int id)
    {
        var result = await _chartOfAccountService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("dropdown")]
    [ProducesResponseType(typeof(List<AccountDropdownDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AccountDropdownDto>>> GetDropdown([FromQuery] AccountType? type = null)
    {
        var result = await _chartOfAccountService.GetDropdownAsync(type);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChartOfAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChartOfAccountDto>> Create([FromBody] CreateChartOfAccountDto dto)
    {
        var result = await _chartOfAccountService.CreateAsync(dto);
        return Created($"api/chart-of-accounts/{result.Id}", result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ChartOfAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChartOfAccountDto>> Update(int id, [FromBody] UpdateChartOfAccountDto dto)
    {
        var result = await _chartOfAccountService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _chartOfAccountService.DeleteAsync(id);
        return NoContent();
    }
}
