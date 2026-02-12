using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Payment;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create a payment (room or banquet)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
    {
        var payment = await _paymentService.CreateAsync(dto);
        return Created($"api/payments/{payment.Id}", payment);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    /// <summary>
    /// Get all payments for a room check-in
    /// </summary>
    [HttpGet("checkin/{checkInId}")]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaymentDto>>> GetByCheckIn(int checkInId)
    {
        var payments = await _paymentService.GetByCheckInIdAsync(checkInId);
        return Ok(payments);
    }

    /// <summary>
    /// Get payment summary for a room check-in
    /// </summary>
    [HttpGet("checkin/{checkInId}/summary")]
    [ProducesResponseType(typeof(PaymentSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSummaryDto>> GetCheckInSummary(int checkInId)
    {
        var summary = await _paymentService.GetPaymentSummaryForCheckInAsync(checkInId);
        return Ok(summary);
    }

    /// <summary>
    /// Get all payments for a banquet booking
    /// </summary>
    [HttpGet("banquet/{bookingId}")]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaymentDto>>> GetByBanquet(int bookingId)
    {
        var payments = await _paymentService.GetByBanquetBookingIdAsync(bookingId);
        return Ok(payments);
    }

    /// <summary>
    /// Get payment summary for a banquet booking
    /// </summary>
    [HttpGet("banquet/{bookingId}/summary")]
    [ProducesResponseType(typeof(PaymentSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSummaryDto>> GetBanquetSummary(int bookingId)
    {
        var summary = await _paymentService.GetPaymentSummaryForBanquetAsync(bookingId);
        return Ok(summary);
    }

    /// <summary>
    /// Get all payments for a company
    /// </summary>
    [HttpGet("company/{companyId}")]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaymentDto>>> GetByCompany(
        int companyId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var payments = await _paymentService.GetByCompanyIdAsync(companyId, from, to);
        return Ok(payments);
    }

    /// <summary>
    /// Cancel/void a payment
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> Cancel(int id, [FromQuery] string? reason = null)
    {
        var payment = await _paymentService.CancelPaymentAsync(id, reason, User.Identity?.Name);
        return Ok(payment);
    }
}
