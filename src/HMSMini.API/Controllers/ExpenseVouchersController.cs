using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.ExpenseVoucher;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/expense-vouchers")]
[Authorize(Roles = "Admin,Manager")]
public class ExpenseVouchersController : ControllerBase
{
    private readonly IExpenseVoucherService _expenseVoucherService;
    private readonly ILogger<ExpenseVouchersController> _logger;

    public ExpenseVouchersController(IExpenseVoucherService expenseVoucherService, ILogger<ExpenseVouchersController> logger)
    {
        _expenseVoucherService = expenseVoucherService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseVoucherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseVoucherDto>> GetById(int id)
    {
        var result = await _expenseVoucherService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseVoucherListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExpenseVoucherListDto>>> GetByDateRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _expenseVoucherService.GetByDateRangeAsync(from, to);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseVoucherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpenseVoucherDto>> Create([FromBody] CreateExpenseVoucherDto dto)
    {
        var result = await _expenseVoucherService.CreateAsync(dto, User.Identity?.Name);
        return Created($"api/expense-vouchers/{result.Id}", result);
    }
}
