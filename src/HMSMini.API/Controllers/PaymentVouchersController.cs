using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.PaymentVoucher;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/payment-vouchers")]
[Authorize(Roles = "Admin,Manager")]
public class PaymentVouchersController : ControllerBase
{
    private readonly IPaymentVoucherService _paymentVoucherService;
    private readonly ILogger<PaymentVouchersController> _logger;

    public PaymentVouchersController(IPaymentVoucherService paymentVoucherService, ILogger<PaymentVouchersController> logger)
    {
        _paymentVoucherService = paymentVoucherService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentVoucherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentVoucherDto>> GetById(int id)
    {
        var result = await _paymentVoucherService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentVoucherListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaymentVoucherListDto>>> GetByDateRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _paymentVoucherService.GetByDateRangeAsync(from, to);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentVoucherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentVoucherDto>> Create([FromBody] CreatePaymentVoucherDto dto)
    {
        var result = await _paymentVoucherService.CreateAsync(dto, User.Identity?.Name);
        return Created($"api/payment-vouchers/{result.Id}", result);
    }
}
