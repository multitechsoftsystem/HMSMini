using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Receipt;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class ReceiptService : IReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly IFinancialYearService _financialYearService;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<ReceiptService> _logger;

    public ReceiptService(
        ApplicationDbContext context,
        IFinancialYearService financialYearService,
        IJournalEntryService journalEntryService,
        IChartOfAccountService chartOfAccountService,
        ILogger<ReceiptService> logger)
    {
        _context = context;
        _financialYearService = financialYearService;
        _journalEntryService = journalEntryService;
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    public async Task<ReceiptDto> GetByIdAsync(int id)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Company)
            .Include(r => r.BankAccount)
            .Include(r => r.Allocations).ThenInclude(a => a.Invoice)
            .Include(r => r.Allocations).ThenInclude(a => a.BanquetInvoice)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null)
            throw new NotFoundException(nameof(Receipt), id);

        return MapToDto(receipt);
    }

    public async Task<List<ReceiptListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Receipts
            .Include(r => r.Company)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(r => r.ReceiptDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(r => r.ReceiptDate <= toDate.Value.Date);

        return await query
            .OrderByDescending(r => r.ReceiptDate)
            .ThenByDescending(r => r.Id)
            .Select(r => new ReceiptListDto
            {
                Id = r.Id,
                ReceiptNumber = r.ReceiptNumber,
                ReceiptDate = r.ReceiptDate,
                ReceivedFrom = r.ReceivedFrom,
                CompanyName = r.Company != null ? r.Company.CompanyName : null,
                Amount = r.Amount,
                PaymentModeName = r.PaymentMode.ToString(),
                CreatedAt = r.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<OutstandingInvoiceDto>> GetOutstandingInvoicesAsync(int? companyId = null)
    {
        var result = new List<OutstandingInvoiceDto>();

        // Room invoices with balance due
        var roomInvoiceQuery = _context.Invoices
            .Where(i => i.BalanceDue > 0);

        if (companyId.HasValue)
            roomInvoiceQuery = roomInvoiceQuery.Where(i => i.CheckIn.CompanyId == companyId.Value);

        var roomInvoices = await roomInvoiceQuery
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new OutstandingInvoiceDto
            {
                InvoiceId = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                GuestNames = i.GuestNames,
                CompanyName = i.CompanyName,
                GrandTotal = i.GrandTotal,
                TotalPaid = i.TotalPaid,
                BalanceDue = i.BalanceDue,
                InvoiceType = "Room"
            })
            .AsNoTracking()
            .ToListAsync();

        result.AddRange(roomInvoices);

        // Banquet invoices with balance due
        var banquetInvoiceQuery = _context.BanquetInvoices
            .Include(i => i.BanquetBooking)
            .Where(i => i.BalanceDue > 0);

        if (companyId.HasValue)
            banquetInvoiceQuery = banquetInvoiceQuery.Where(i => i.BanquetBooking.CompanyId == companyId.Value);

        var banquetInvoices = await banquetInvoiceQuery
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new OutstandingInvoiceDto
            {
                InvoiceId = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                GuestNames = i.ContactPersonName,
                CompanyName = i.CompanyName,
                GrandTotal = i.GrandTotal,
                TotalPaid = i.TotalPaid,
                BalanceDue = i.BalanceDue,
                InvoiceType = "Banquet"
            })
            .AsNoTracking()
            .ToListAsync();

        result.AddRange(banquetInvoices);

        return result.OrderBy(r => r.InvoiceDate).ToList();
    }

    public async Task<ReceiptDto> CreateAsync(CreateReceiptDto dto, string? createdBy = null)
    {
        if (dto.Amount <= 0)
            throw new BusinessRuleException("Amount must be greater than zero.");

        var fyId = await _financialYearService.GetCurrentFinancialYearIdAsync();

        // Validate allocations total
        var allocatedTotal = dto.Allocations.Sum(a => a.AllocatedAmount);
        if (allocatedTotal > dto.Amount)
            throw new BusinessRuleException("Allocated amount cannot exceed receipt amount.");

        // Determine accounts
        int debitAccountId;
        if (dto.PaymentMode == PaymentMode.Cash)
        {
            debitAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1001"); // Cash
        }
        else
        {
            debitAccountId = dto.BankAccountId ?? await _chartOfAccountService.GetAccountIdByCodeAsync("1002"); // Bank
        }
        int creditAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1003"); // Accounts Receivable

        var receiptNumber = await GenerateReceiptNumberAsync(dto.ReceiptDate);

        var receipt = new Receipt
        {
            ReceiptNumber = receiptNumber,
            ReceiptDate = dto.ReceiptDate,
            FinancialYearId = fyId,
            ReceivedFrom = dto.ReceivedFrom,
            CompanyId = dto.CompanyId,
            Amount = dto.Amount,
            PaymentMode = dto.PaymentMode,
            BankAccountId = dto.BankAccountId,
            ReferenceNumber = dto.ReferenceNumber,
            Narration = dto.Narration,
            CreatedBy = createdBy
        };

        // Add allocations
        foreach (var alloc in dto.Allocations)
        {
            receipt.Allocations.Add(new ReceiptAllocation
            {
                InvoiceId = alloc.InvoiceId,
                BanquetInvoiceId = alloc.BanquetInvoiceId,
                AllocatedAmount = alloc.AllocatedAmount
            });
        }

        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync();

        // Update invoice TotalPaid/BalanceDue for each allocation
        foreach (var alloc in dto.Allocations)
        {
            if (alloc.InvoiceId.HasValue)
            {
                var invoice = await _context.Invoices.FindAsync(alloc.InvoiceId.Value);
                if (invoice != null)
                {
                    invoice.TotalPaid += alloc.AllocatedAmount;
                    invoice.BalanceDue = invoice.GrandTotal - invoice.TotalPaid;
                    invoice.PaymentStatus = invoice.BalanceDue <= 0 ? "Paid"
                        : invoice.TotalPaid > 0 ? "PartiallyPaid"
                        : "Unpaid";
                }
            }
            if (alloc.BanquetInvoiceId.HasValue)
            {
                var bInvoice = await _context.BanquetInvoices.FindAsync(alloc.BanquetInvoiceId.Value);
                if (bInvoice != null)
                {
                    bInvoice.TotalPaid += alloc.AllocatedAmount;
                    bInvoice.BalanceDue = bInvoice.GrandTotal - bInvoice.TotalPaid;
                    bInvoice.PaymentStatus = bInvoice.BalanceDue <= 0 ? "Paid"
                        : bInvoice.TotalPaid > 0 ? "PartiallyPaid"
                        : "Unpaid";
                }
            }
        }

        await _context.SaveChangesAsync();

        // Create journal entry: Dr. Cash or Bank / Cr. Accounts Receivable
        try
        {
            var lines = new List<(int accountId, decimal debit, decimal credit, string? desc)>
            {
                (debitAccountId, dto.Amount, 0, $"Receipt from {dto.ReceivedFrom}"),
                (creditAccountId, 0, dto.Amount, $"Outstanding cleared - {dto.ReceivedFrom}")
            };

            var jeId = await _journalEntryService.PostJournalEntryAsync(
                dto.ReceiptDate,
                $"Receipt {receiptNumber}: {dto.ReceivedFrom}",
                JournalSourceType.Receipt,
                receipt.Id,
                lines,
                createdBy);

            receipt.JournalEntryId = jeId;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create journal entry for receipt {ReceiptNumber}", receiptNumber);
        }

        _logger.LogInformation("Created receipt {ReceiptNumber} for {Amount} from {ReceivedFrom}",
            receiptNumber, dto.Amount, dto.ReceivedFrom);
        return await GetByIdAsync(receipt.Id);
    }

    private async Task<string> GenerateReceiptNumberAsync(DateTime date)
    {
        var prefix = $"REC-{date:yyyyMMdd}-";

        var last = await _context.Receipts
            .IgnoreQueryFilters()
            .Where(r => r.ReceiptNumber.StartsWith(prefix))
            .OrderByDescending(r => r.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (last != null)
        {
            var lastStr = last.ReceiptNumber.Substring(prefix.Length);
            if (int.TryParse(lastStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    private static ReceiptDto MapToDto(Receipt r)
    {
        return new ReceiptDto
        {
            Id = r.Id,
            ReceiptNumber = r.ReceiptNumber,
            ReceiptDate = r.ReceiptDate,
            FinancialYearId = r.FinancialYearId,
            ReceivedFrom = r.ReceivedFrom,
            CompanyId = r.CompanyId,
            CompanyName = r.Company?.CompanyName,
            Amount = r.Amount,
            PaymentMode = r.PaymentMode,
            BankAccountId = r.BankAccountId,
            BankAccountName = r.BankAccount != null ? $"{r.BankAccount.AccountCode} - {r.BankAccount.AccountName}" : null,
            ReferenceNumber = r.ReferenceNumber,
            Narration = r.Narration,
            JournalEntryId = r.JournalEntryId,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            Allocations = r.Allocations.Select(a => new ReceiptAllocationDto
            {
                Id = a.Id,
                ReceiptId = a.ReceiptId,
                InvoiceId = a.InvoiceId,
                InvoiceNumber = a.Invoice?.InvoiceNumber,
                BanquetInvoiceId = a.BanquetInvoiceId,
                BanquetInvoiceNumber = a.BanquetInvoice?.InvoiceNumber,
                AllocatedAmount = a.AllocatedAmount
            }).ToList()
        };
    }
}
