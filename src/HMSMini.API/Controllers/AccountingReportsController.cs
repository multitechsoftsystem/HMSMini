using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.AccountingReport;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/accounting-reports")]
[Authorize(Roles = "Admin,Manager")]
public class AccountingReportsController : ControllerBase
{
    private readonly IAccountingReportService _accountingReportService;
    private readonly ILogger<AccountingReportsController> _logger;

    public AccountingReportsController(IAccountingReportService accountingReportService, ILogger<AccountingReportsController> logger)
    {
        _accountingReportService = accountingReportService;
        _logger = logger;
    }

    [HttpGet("trial-balance")]
    [ProducesResponseType(typeof(TrialBalanceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TrialBalanceDto>> GetTrialBalance(
        [FromQuery] int? financialYearId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _accountingReportService.GetTrialBalanceAsync(financialYearId, from, to);
        return Ok(result);
    }

    [HttpGet("balance-sheet")]
    [ProducesResponseType(typeof(BalanceSheetDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BalanceSheetDto>> GetBalanceSheet(
        [FromQuery] DateTime? asOfDate = null,
        [FromQuery] int? financialYearId = null)
    {
        var result = await _accountingReportService.GetBalanceSheetAsync(asOfDate, financialYearId);
        return Ok(result);
    }
}
