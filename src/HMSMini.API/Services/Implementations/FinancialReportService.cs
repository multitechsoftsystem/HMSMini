using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.FinancialReport;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class FinancialReportService : IFinancialReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FinancialReportService> _logger;

    public FinancialReportService(ApplicationDbContext context, ILogger<FinancialReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CompanyOutstandingListDto> GetCompanyOutstandingSummaryAsync(DateTime? asOfDate = null)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        var companies = await _context.Companies
            .Where(c => c.IsActive && c.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        var result = new CompanyOutstandingListDto
        {
            AsOfDate = effectiveDate
        };

        foreach (var company in companies)
        {
            var summary = await CalculateCompanyOutstandingAsync(company.Id, company.CompanyName, effectiveDate);

            if (summary.TotalCharged > 0 || summary.TotalPaid > 0)
            {
                result.Companies.Add(summary);
            }
        }

        result.GrandTotalCharged = result.Companies.Sum(c => c.TotalCharged);
        result.GrandTotalPaid = result.Companies.Sum(c => c.TotalPaid);
        result.GrandTotalOutstanding = result.Companies.Sum(c => c.Outstanding);

        return result;
    }

    public async Task<CompanyOutstandingSummaryDto> GetCompanyOutstandingByIdAsync(int companyId, DateTime? asOfDate = null)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Models.Entities.Company), companyId);

        var effectiveDate = asOfDate ?? DateTime.UtcNow;
        return await CalculateCompanyOutstandingAsync(companyId, company.CompanyName, effectiveDate);
    }

    public async Task<CompanyStatementDto> GetCompanyStatementAsync(int companyId, DateTime fromDate, DateTime toDate)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Models.Entities.Company), companyId);

        var lineItems = new List<StatementLineItemDto>();

        // Get room invoices in date range
        var roomInvoices = await _context.Invoices
            .Where(i => i.CheckIn.CompanyId == companyId
                     && i.InvoiceDate >= fromDate.Date
                     && i.InvoiceDate <= toDate.Date
                     && i.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        foreach (var inv in roomInvoices)
        {
            lineItems.Add(new StatementLineItemDto
            {
                Date = inv.InvoiceDate,
                TransactionType = "Invoice",
                SourceType = PaymentSourceType.Room,
                Description = $"Room Invoice {inv.InvoiceNumber} - Room {inv.RoomNumber} ({inv.GuestNames})",
                ChargeAmount = inv.GrandTotal,
                PaymentAmount = 0
            });
        }

        // Get banquet invoices in date range
        var banquetInvoices = await _context.BanquetInvoices
            .Where(i => i.BanquetBooking.CompanyId == companyId
                     && i.InvoiceDate >= fromDate.Date
                     && i.InvoiceDate <= toDate.Date
                     && i.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        foreach (var inv in banquetInvoices)
        {
            lineItems.Add(new StatementLineItemDto
            {
                Date = inv.InvoiceDate,
                TransactionType = "Invoice",
                SourceType = PaymentSourceType.Banquet,
                Description = $"Banquet Invoice {inv.InvoiceNumber} - {inv.HallName} ({inv.EventTypeName})",
                ChargeAmount = inv.GrandTotal,
                PaymentAmount = 0
            });
        }

        // Get voucher charges for active (non-invoiced) check-ins in date range
        var activeCheckInIds = await _context.CheckIns
            .Where(c => c.CompanyId == companyId
                     && c.Status == CheckInStatus.Active
                     && c.DeletedAt == null)
            .Select(c => c.Id)
            .ToListAsync();

        if (activeCheckInIds.Any())
        {
            var voucherCharges = await _context.Vouchers
                .Where(v => activeCheckInIds.Contains(v.CheckInId!.Value)
                         && v.PostingStatus == "Posted"
                         && v.VoucherDate >= fromDate.Date
                         && v.VoucherDate <= toDate.Date)
                .AsNoTracking()
                .ToListAsync();

            foreach (var v in voucherCharges)
            {
                if (v.VoucherType != VoucherType.Payment && v.VoucherType != VoucherType.Refund)
                {
                    lineItems.Add(new StatementLineItemDto
                    {
                        Date = v.VoucherDate,
                        TransactionType = "Voucher",
                        SourceType = PaymentSourceType.Room,
                        Description = $"{v.VoucherType} - Room {v.RoomNumber} ({v.VoucherNumber})",
                        ChargeAmount = v.Amount > 0 ? v.Amount : 0,
                        PaymentAmount = v.Amount < 0 ? -v.Amount : 0
                    });
                }
            }
        }

        // Get payments in date range
        var payments = await _context.Payments
            .Where(p => p.CompanyId == companyId
                     && p.PaymentDate >= fromDate.Date
                     && p.PaymentDate <= toDate.Date)
            .AsNoTracking()
            .ToListAsync();

        foreach (var p in payments)
        {
            var isRefund = p.PaymentType == PaymentType.Refund;
            lineItems.Add(new StatementLineItemDto
            {
                Date = p.PaymentDate,
                TransactionType = isRefund ? "Refund" : "Payment",
                SourceType = p.SourceType,
                Description = $"{(isRefund ? "Refund" : "Payment")} {p.ReceiptNumber} ({p.PaymentMode})",
                ChargeAmount = isRefund ? p.Amount : 0,
                PaymentAmount = isRefund ? 0 : p.Amount
            });
        }

        // Sort by date and calculate running balance
        lineItems = lineItems.OrderBy(l => l.Date).ThenBy(l => l.TransactionType).ToList();

        // Calculate opening balance (all charges - payments before fromDate)
        var openingBalance = await CalculateBalanceBeforeDateAsync(companyId, fromDate);

        decimal runningBalance = openingBalance;
        foreach (var item in lineItems)
        {
            runningBalance += item.ChargeAmount - item.PaymentAmount;
            item.RunningBalance = runningBalance;
        }

        return new CompanyStatementDto
        {
            CompanyId = companyId,
            CompanyName = company.CompanyName,
            FromDate = fromDate,
            ToDate = toDate,
            OpeningBalance = openingBalance,
            TotalCharges = lineItems.Sum(l => l.ChargeAmount),
            TotalPayments = lineItems.Sum(l => l.PaymentAmount),
            ClosingBalance = runningBalance,
            LineItems = lineItems
        };
    }

    public async Task<AgingReportDto> GetAgingReportAsync(DateTime? asOfDate = null)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        var companies = await _context.Companies
            .Where(c => c.IsActive && c.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        var report = new AgingReportDto
        {
            AsOfDate = effectiveDate
        };

        foreach (var company in companies)
        {
            var bucket = await CalculateAgingBucketAsync(company.Id, company.CompanyName, effectiveDate);
            if (bucket.TotalOutstanding > 0)
            {
                report.Companies.Add(bucket);
            }
        }

        report.TotalCurrent = report.Companies.Sum(c => c.Current);
        report.TotalDays31To60 = report.Companies.Sum(c => c.Days31To60);
        report.TotalDays61To90 = report.Companies.Sum(c => c.Days61To90);
        report.TotalOver90Days = report.Companies.Sum(c => c.Over90Days);
        report.GrandTotal = report.Companies.Sum(c => c.TotalOutstanding);

        return report;
    }

    public async Task<BusinessSourceOutstandingDto> GetBusinessSourceOutstandingAsync(DateTime? asOfDate = null)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        var sources = await _context.BusinessSources
            .Where(s => s.IsActive && s.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        var result = new BusinessSourceOutstandingDto
        {
            AsOfDate = effectiveDate
        };

        foreach (var source in sources)
        {
            // Get check-ins with this business source that have company billing
            var checkInIds = await _context.CheckIns
                .Where(c => c.BusinessSourceId == source.BusinessSourceId
                         && c.CompanyId != null
                         && c.DeletedAt == null)
                .Select(c => c.Id)
                .ToListAsync();

            if (!checkInIds.Any())
                continue;

            // Room charges from invoices
            var roomInvoiceTotal = await _context.Invoices
                .Where(i => checkInIds.Contains(i.CheckInId) && i.DeletedAt == null)
                .SumAsync(i => i.GrandTotal);

            // Room charges from vouchers for active check-ins
            var activeCheckInIds = await _context.CheckIns
                .Where(c => checkInIds.Contains(c.Id) && c.Status == CheckInStatus.Active)
                .Select(c => c.Id)
                .ToListAsync();

            var voucherTotal = 0m;
            if (activeCheckInIds.Any())
            {
                voucherTotal = await _context.Vouchers
                    .Where(v => v.CheckInId.HasValue
                             && activeCheckInIds.Contains(v.CheckInId.Value)
                             && v.PostingStatus == "Posted"
                             && v.VoucherType != VoucherType.Payment
                             && v.VoucherType != VoucherType.Refund)
                    .SumAsync(v => v.Amount);
            }

            var totalCharged = roomInvoiceTotal + voucherTotal;

            // Payments for these check-ins
            var totalPaid = await _context.Payments
                .Where(p => p.CheckInId.HasValue && checkInIds.Contains(p.CheckInId.Value))
                .SumAsync(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

            if (totalCharged > 0 || totalPaid > 0)
            {
                result.Sources.Add(new BusinessSourceOutstandingItemDto
                {
                    BusinessSourceId = source.BusinessSourceId,
                    BusinessSourceName = source.SourceName,
                    TotalCharged = totalCharged,
                    TotalPaid = totalPaid,
                    Outstanding = totalCharged - totalPaid
                });
            }
        }

        result.GrandTotalCharged = result.Sources.Sum(s => s.TotalCharged);
        result.GrandTotalPaid = result.Sources.Sum(s => s.TotalPaid);
        result.GrandTotalOutstanding = result.Sources.Sum(s => s.Outstanding);

        return result;
    }

    private async Task<CompanyOutstandingSummaryDto> CalculateCompanyOutstandingAsync(
        int companyId, string companyName, DateTime asOfDate)
    {
        // Room charges: invoices + voucher totals for active check-ins
        var roomInvoiceTotal = await _context.Invoices
            .Where(i => i.CheckIn.CompanyId == companyId
                     && i.InvoiceDate <= asOfDate
                     && i.DeletedAt == null)
            .SumAsync(i => i.GrandTotal);

        // Active check-in voucher charges (not yet invoiced)
        var activeCheckInIds = await _context.CheckIns
            .Where(c => c.CompanyId == companyId
                     && c.Status == CheckInStatus.Active
                     && c.DeletedAt == null)
            .Select(c => c.Id)
            .ToListAsync();

        var activeVoucherTotal = 0m;
        if (activeCheckInIds.Any())
        {
            activeVoucherTotal = await _context.Vouchers
                .Where(v => v.CheckInId.HasValue
                         && activeCheckInIds.Contains(v.CheckInId.Value)
                         && v.PostingStatus == "Posted"
                         && v.VoucherDate <= asOfDate
                         && v.VoucherType != VoucherType.Payment
                         && v.VoucherType != VoucherType.Refund)
                .SumAsync(v => v.Amount);
        }

        var roomCharges = roomInvoiceTotal + activeVoucherTotal;

        // Banquet charges: from banquet invoices
        var banquetCharges = await _context.BanquetInvoices
            .Where(i => i.BanquetBooking.CompanyId == companyId
                     && i.InvoiceDate <= asOfDate
                     && i.DeletedAt == null)
            .SumAsync(i => i.GrandTotal);

        // Total payments for this company
        var totalPaid = await _context.Payments
            .Where(p => p.CompanyId == companyId
                     && p.PaymentDate <= asOfDate)
            .SumAsync(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        var totalCharged = roomCharges + banquetCharges;

        return new CompanyOutstandingSummaryDto
        {
            CompanyId = companyId,
            CompanyName = companyName,
            RoomCharges = roomCharges,
            BanquetCharges = banquetCharges,
            TotalCharged = totalCharged,
            TotalPaid = totalPaid,
            Outstanding = totalCharged - totalPaid
        };
    }

    private async Task<decimal> CalculateBalanceBeforeDateAsync(int companyId, DateTime beforeDate)
    {
        // Room invoice charges before date
        var roomCharges = await _context.Invoices
            .Where(i => i.CheckIn.CompanyId == companyId
                     && i.InvoiceDate < beforeDate.Date
                     && i.DeletedAt == null)
            .SumAsync(i => i.GrandTotal);

        // Banquet charges before date
        var banquetCharges = await _context.BanquetInvoices
            .Where(i => i.BanquetBooking.CompanyId == companyId
                     && i.InvoiceDate < beforeDate.Date
                     && i.DeletedAt == null)
            .SumAsync(i => i.GrandTotal);

        // Voucher charges for active check-ins before date
        var activeCheckInIds = await _context.CheckIns
            .Where(c => c.CompanyId == companyId
                     && c.Status == CheckInStatus.Active
                     && c.DeletedAt == null)
            .Select(c => c.Id)
            .ToListAsync();

        var voucherCharges = 0m;
        if (activeCheckInIds.Any())
        {
            voucherCharges = await _context.Vouchers
                .Where(v => v.CheckInId.HasValue
                         && activeCheckInIds.Contains(v.CheckInId.Value)
                         && v.PostingStatus == "Posted"
                         && v.VoucherDate < beforeDate.Date
                         && v.VoucherType != VoucherType.Payment
                         && v.VoucherType != VoucherType.Refund)
                .SumAsync(v => v.Amount);
        }

        // Payments before date
        var payments = await _context.Payments
            .Where(p => p.CompanyId == companyId
                     && p.PaymentDate < beforeDate.Date)
            .SumAsync(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        return (roomCharges + banquetCharges + voucherCharges) - payments;
    }

    private async Task<AgingBucketDto> CalculateAgingBucketAsync(
        int companyId, string companyName, DateTime asOfDate)
    {
        var bucket = new AgingBucketDto
        {
            CompanyId = companyId,
            CompanyName = companyName
        };

        // Get all unpaid invoices with their amounts
        var roomInvoices = await _context.Invoices
            .Where(i => i.CheckIn.CompanyId == companyId
                     && i.DeletedAt == null
                     && i.InvoiceDate <= asOfDate)
            .Select(i => new { i.InvoiceDate, i.GrandTotal, i.TotalPaid })
            .AsNoTracking()
            .ToListAsync();

        foreach (var inv in roomInvoices)
        {
            var outstanding = inv.GrandTotal - inv.TotalPaid;
            if (outstanding <= 0) continue;

            var daysPast = (asOfDate - inv.InvoiceDate).Days;
            AddToAgingBucket(bucket, outstanding, daysPast);
        }

        var banquetInvoices = await _context.BanquetInvoices
            .Where(i => i.BanquetBooking.CompanyId == companyId
                     && i.DeletedAt == null
                     && i.InvoiceDate <= asOfDate)
            .Select(i => new { i.InvoiceDate, i.GrandTotal, i.TotalPaid })
            .AsNoTracking()
            .ToListAsync();

        foreach (var inv in banquetInvoices)
        {
            var outstanding = inv.GrandTotal - inv.TotalPaid;
            if (outstanding <= 0) continue;

            var daysPast = (asOfDate - inv.InvoiceDate).Days;
            AddToAgingBucket(bucket, outstanding, daysPast);
        }

        bucket.TotalOutstanding = bucket.Current + bucket.Days31To60 + bucket.Days61To90 + bucket.Over90Days;

        return bucket;
    }

    private static void AddToAgingBucket(AgingBucketDto bucket, decimal amount, int daysPast)
    {
        if (daysPast <= 30)
            bucket.Current += amount;
        else if (daysPast <= 60)
            bucket.Days31To60 += amount;
        else if (daysPast <= 90)
            bucket.Days61To90 += amount;
        else
            bucket.Over90Days += amount;
    }
}
