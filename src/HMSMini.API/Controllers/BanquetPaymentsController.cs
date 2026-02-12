using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetPayment;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-payments")]
[Authorize]
[Obsolete("Use PaymentsController instead. These endpoints will be removed in a future version.")]
public class BanquetPaymentsController : ControllerBase
{
    private readonly IBanquetPaymentService _paymentService;
    private readonly ILogger<BanquetPaymentsController> _logger;

    public BanquetPaymentsController(IBanquetPaymentService paymentService, ILogger<BanquetPaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<BanquetPaymentDto>>> GetByBooking(int bookingId)
    {
        var payments = await _paymentService.GetByBookingAsync(bookingId);
        return Ok(payments);
    }

    [HttpPost("booking/{bookingId}")]
    public async Task<ActionResult<BanquetPaymentDto>> Create(int bookingId, [FromBody] CreateBanquetPaymentDto dto)
    {
        var payment = await _paymentService.CreateAsync(bookingId, dto);
        return Created($"api/banquet-payments/{payment.Id}", payment);
    }

    [HttpGet("booking/{bookingId}/summary")]
    public async Task<ActionResult<BanquetPaymentSummaryDto>> GetSummary(int bookingId)
    {
        var summary = await _paymentService.GetPaymentSummaryAsync(bookingId);
        return Ok(summary);
    }
}
