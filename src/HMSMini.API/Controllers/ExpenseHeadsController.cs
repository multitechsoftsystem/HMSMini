using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.ExpenseHead;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/expense-heads")]
[Authorize(Roles = "Admin,Manager")]
public class ExpenseHeadsController : ControllerBase
{
    private readonly IExpenseHeadService _expenseHeadService;
    private readonly ILogger<ExpenseHeadsController> _logger;

    public ExpenseHeadsController(IExpenseHeadService expenseHeadService, ILogger<ExpenseHeadsController> logger)
    {
        _expenseHeadService = expenseHeadService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseHeadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExpenseHeadDto>>> GetAll()
    {
        var result = await _expenseHeadService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseHeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseHeadDto>> GetById(int id)
    {
        var result = await _expenseHeadService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseHeadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpenseHeadDto>> Create([FromBody] CreateExpenseHeadDto dto)
    {
        var result = await _expenseHeadService.CreateAsync(dto);
        return Created($"api/expense-heads/{result.Id}", result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExpenseHeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseHeadDto>> Update(int id, [FromBody] UpdateExpenseHeadDto dto)
    {
        var result = await _expenseHeadService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _expenseHeadService.DeleteAsync(id);
        return NoContent();
    }
}
