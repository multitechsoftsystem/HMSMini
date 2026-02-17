using HMSMini.API.Models.DTOs.AccountingReport;

namespace HMSMini.API.Services.Interfaces;

public interface IAccountingReportService
{
    Task<TrialBalanceDto> GetTrialBalanceAsync(int? financialYearId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime? asOfDate = null, int? financialYearId = null);
}
