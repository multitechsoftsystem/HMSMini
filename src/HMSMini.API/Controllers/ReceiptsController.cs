using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Receipt;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/receipts")]
[Authorize(Roles = "Admin,Manager")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly ILogger<ReceiptsController> _logger;

    public ReceiptsController(IReceiptService receiptService, ILogger<ReceiptsController> logger)
    {
        _receiptService = receiptService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReceiptDto>> GetById(int id)
    {
        var result = await _receiptService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ReceiptListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReceiptListDto>>> GetByDateRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _receiptService.GetByDateRangeAsync(from, to);
        return Ok(result);
    }

    [HttpGet("outstanding-invoices")]
    [ProducesResponseType(typeof(List<OutstandingInvoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OutstandingInvoiceDto>>> GetOutstandingInvoices(
        [FromQuery] int? companyId = null)
    {
        var result = await _receiptService.GetOutstandingInvoicesAsync(companyId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReceiptDto>> Create([FromBody] CreateReceiptDto dto)
    {
        var result = await _receiptService.CreateAsync(dto, User.Identity?.Name);
        return Created($"api/receipts/{result.Id}", result);
    }
}
