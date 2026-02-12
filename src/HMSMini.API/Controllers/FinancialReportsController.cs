using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.FinancialReport;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/financial-reports")]
[Authorize(Roles = "Admin,Manager")]
public class FinancialReportsController : ControllerBase
{
    private readonly IFinancialReportService _reportService;
    private readonly ILogger<FinancialReportsController> _logger;

    public FinancialReportsController(IFinancialReportService reportService, ILogger<FinancialReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Get outstanding amounts for all companies
    /// </summary>
    [HttpGet("company-outstanding")]
    [ProducesResponseType(typeof(CompanyOutstandingListDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyOutstandingListDto>> GetCompanyOutstanding([FromQuery] DateTime? asOfDate = null)
    {
        var result = await _reportService.GetCompanyOutstandingSummaryAsync(asOfDate);
        return Ok(result);
    }

    /// <summary>
    /// Get outstanding amount for a single company
    /// </summary>
    [HttpGet("company-outstanding/{companyId}")]
    [ProducesResponseType(typeof(CompanyOutstandingSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyOutstandingSummaryDto>> GetCompanyOutstandingById(
        int companyId,
        [FromQuery] DateTime? asOfDate = null)
    {
        var result = await _reportService.GetCompanyOutstandingByIdAsync(companyId, asOfDate);
        return Ok(result);
    }

    /// <summary>
    /// Get company statement with chronological line items
    /// </summary>
    [HttpGet("company-statement/{companyId}")]
    [ProducesResponseType(typeof(CompanyStatementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyStatementDto>> GetCompanyStatement(
        int companyId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var result = await _reportService.GetCompanyStatementAsync(companyId, from, to);
        return Ok(result);
    }

    /// <summary>
    /// Get aging report (0-30, 31-60, 61-90, 90+ days)
    /// </summary>
    [HttpGet("aging")]
    [ProducesResponseType(typeof(AgingReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgingReportDto>> GetAgingReport([FromQuery] DateTime? asOfDate = null)
    {
        var result = await _reportService.GetAgingReportAsync(asOfDate);
        return Ok(result);
    }

    /// <summary>
    /// Get outstanding amounts by business source
    /// </summary>
    [HttpGet("business-source-outstanding")]
    [ProducesResponseType(typeof(BusinessSourceOutstandingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BusinessSourceOutstandingDto>> GetBusinessSourceOutstanding([FromQuery] DateTime? asOfDate = null)
    {
        var result = await _reportService.GetBusinessSourceOutstandingAsync(asOfDate);
        return Ok(result);
    }
}
