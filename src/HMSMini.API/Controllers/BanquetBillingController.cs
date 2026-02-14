using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetBilling;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-billing")]
[Authorize]
public class BanquetBillingController : ControllerBase
{
    private readonly IBanquetBillingService _billingService;
    private readonly ILogger<BanquetBillingController> _logger;

    public BanquetBillingController(IBanquetBillingService billingService, ILogger<BanquetBillingController> logger)
    {
        _billingService = billingService;
        _logger = logger;
    }

    [HttpGet("booking/{bookingId}/preview")]
    public async Task<ActionResult<BanquetBillPreviewDto>> GetBillPreview(int bookingId)
    {
        var preview = await _billingService.GetBillPreviewAsync(bookingId);
        return Ok(preview);
    }

    [HttpPost("booking/{bookingId}/finalize")]
    [Authorize(Roles = "Admin,Manager,BanquetManager,BanquetStaff")]
    public async Task<ActionResult<BanquetInvoiceDto>> FinalizeInvoice(int bookingId, [FromBody] FinalizeBanquetInvoiceDto dto)
    {
        var invoice = await _billingService.FinalizeInvoiceAsync(bookingId, dto);
        return CreatedAtAction(nameof(GetInvoiceById), new { invoiceId = invoice.Id }, invoice);
    }

    [HttpGet("invoice/{invoiceId}")]
    public async Task<ActionResult<BanquetInvoiceDto>> GetInvoiceById(int invoiceId)
    {
        var invoice = await _billingService.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null) return NotFound($"Banquet invoice with ID {invoiceId} not found.");
        return Ok(invoice);
    }

    [HttpGet("booking/{bookingId}/invoice")]
    public async Task<ActionResult<BanquetInvoiceDto>> GetInvoiceByBooking(int bookingId)
    {
        var invoice = await _billingService.GetInvoiceByBookingIdAsync(bookingId);
        if (invoice == null) return NotFound($"No invoice found for booking {bookingId}.");
        return Ok(invoice);
    }
}
