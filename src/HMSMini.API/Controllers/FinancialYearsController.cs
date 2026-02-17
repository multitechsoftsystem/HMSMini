using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.FinancialYear;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/financial-years")]
[Authorize(Roles = "Admin,Manager")]
public class FinancialYearsController : ControllerBase
{
    private readonly IFinancialYearService _financialYearService;
    private readonly ILogger<FinancialYearsController> _logger;

    public FinancialYearsController(IFinancialYearService financialYearService, ILogger<FinancialYearsController> logger)
    {
        _financialYearService = financialYearService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<FinancialYearDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialYearDto>>> GetAll()
    {
        var result = await _financialYearService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FinancialYearDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinancialYearDto>> GetById(int id)
    {
        var result = await _financialYearService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(FinancialYearDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinancialYearDto>> GetCurrent()
    {
        var result = await _financialYearService.GetCurrentAsync();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FinancialYearDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FinancialYearDto>> Create([FromBody] CreateFinancialYearDto dto)
    {
        var result = await _financialYearService.CreateAsync(dto);
        return Created($"api/financial-years/{result.Id}", result);
    }

    [HttpPut("{id}/set-current")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetCurrent(int id)
    {
        await _financialYearService.SetCurrentAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(int id)
    {
        await _financialYearService.CloseAsync(id, User.Identity?.Name);
        return NoContent();
    }
}
