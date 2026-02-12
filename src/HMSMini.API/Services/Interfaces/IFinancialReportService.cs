using HMSMini.API.Models.DTOs.FinancialReport;

namespace HMSMini.API.Services.Interfaces;

public interface IFinancialReportService
{
    Task<CompanyOutstandingListDto> GetCompanyOutstandingSummaryAsync(DateTime? asOfDate = null);
    Task<CompanyOutstandingSummaryDto> GetCompanyOutstandingByIdAsync(int companyId, DateTime? asOfDate = null);
    Task<CompanyStatementDto> GetCompanyStatementAsync(int companyId, DateTime fromDate, DateTime toDate);
    Task<AgingReportDto> GetAgingReportAsync(DateTime? asOfDate = null);
    Task<BusinessSourceOutstandingDto> GetBusinessSourceOutstandingAsync(DateTime? asOfDate = null);
}
