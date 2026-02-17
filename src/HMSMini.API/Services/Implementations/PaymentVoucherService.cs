using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.PaymentVoucher;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class PaymentVoucherService : IPaymentVoucherService
{
    private readonly ApplicationDbContext _context;
    private readonly IFinancialYearService _financialYearService;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<PaymentVoucherService> _logger;

    public PaymentVoucherService(
        ApplicationDbContext context,
        IFinancialYearService financialYearService,
        IJournalEntryService journalEntryService,
        IChartOfAccountService chartOfAccountService,
        ILogger<PaymentVoucherService> logger)
    {
        _context = context;
        _financialYearService = financialYearService;
        _journalEntryService = journalEntryService;
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    public async Task<PaymentVoucherDto> GetByIdAsync(int id)
    {
        var voucher = await _context.PaymentVouchers
            .Include(v => v.BankAccount)
            .Include(v => v.ExpenseVoucher)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voucher == null)
            throw new NotFoundException(nameof(PaymentVoucher), id);

        return MapToDto(voucher);
    }

    public async Task<List<PaymentVoucherListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.PaymentVouchers.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(v => v.VoucherDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(v => v.VoucherDate <= toDate.Value.Date);

        return await query
            .OrderByDescending(v => v.VoucherDate)
            .ThenByDescending(v => v.Id)
            .Select(v => new PaymentVoucherListDto
            {
                Id = v.Id,
                VoucherNumber = v.VoucherNumber,
                VoucherDate = v.VoucherDate,
                PayeeName = v.PayeeName,
                Amount = v.Amount,
                PaymentModeName = v.PaymentMode.ToString(),
                CreatedAt = v.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PaymentVoucherDto> CreateAsync(CreatePaymentVoucherDto dto, string? createdBy = null)
    {
        if (dto.Amount <= 0)
            throw new BusinessRuleException("Amount must be greater than zero.");

        var fyId = await _financialYearService.GetCurrentFinancialYearIdAsync();

        // Determine accounts
        int debitAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("2001"); // Accounts Payable
        int creditAccountId;
        if (dto.PaymentMode == PaymentMode.Cash)
        {
            creditAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1001"); // Cash
        }
        else
        {
            creditAccountId = dto.BankAccountId ?? await _chartOfAccountService.GetAccountIdByCodeAsync("1002"); // Bank
        }

        var voucherNumber = await GenerateVoucherNumberAsync(dto.VoucherDate);

        var voucher = new PaymentVoucher
        {
            VoucherNumber = voucherNumber,
            VoucherDate = dto.VoucherDate,
            FinancialYearId = fyId,
            PayeeName = dto.PayeeName,
            Amount = dto.Amount,
            PaymentMode = dto.PaymentMode,
            BankAccountId = dto.BankAccountId,
            ReferenceNumber = dto.ReferenceNumber,
            Narration = dto.Narration,
            ExpenseVoucherId = dto.ExpenseVoucherId,
            CreatedBy = createdBy
        };

        _context.PaymentVouchers.Add(voucher);
        await _context.SaveChangesAsync();

        // Create journal entry: Dr. Accounts Payable / Cr. Cash or Bank
        try
        {
            var lines = new List<(int accountId, decimal debit, decimal credit, string? desc)>
            {
                (debitAccountId, dto.Amount, 0, $"Payment to {dto.PayeeName}"),
                (creditAccountId, 0, dto.Amount, $"Vendor payment - {dto.PayeeName}")
            };

            var jeId = await _journalEntryService.PostJournalEntryAsync(
                dto.VoucherDate,
                $"Payment Voucher {voucherNumber}: {dto.PayeeName}",
                JournalSourceType.PaymentVoucher,
                voucher.Id,
                lines,
                createdBy);

            voucher.JournalEntryId = jeId;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create journal entry for payment voucher {VoucherNumber}", voucherNumber);
        }

        _logger.LogInformation("Created payment voucher {VoucherNumber} for {Amount} to {Payee}",
            voucherNumber, dto.Amount, dto.PayeeName);
        return await GetByIdAsync(voucher.Id);
    }

    private async Task<string> GenerateVoucherNumberAsync(DateTime date)
    {
        var prefix = $"PV-{date:yyyyMMdd}-";

        var last = await _context.PaymentVouchers
            .IgnoreQueryFilters()
            .Where(v => v.VoucherNumber.StartsWith(prefix))
            .OrderByDescending(v => v.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (last != null)
        {
            var lastStr = last.VoucherNumber.Substring(prefix.Length);
            if (int.TryParse(lastStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    private static PaymentVoucherDto MapToDto(PaymentVoucher v)
    {
        return new PaymentVoucherDto
        {
            Id = v.Id,
            VoucherNumber = v.VoucherNumber,
            VoucherDate = v.VoucherDate,
            FinancialYearId = v.FinancialYearId,
            PayeeName = v.PayeeName,
            Amount = v.Amount,
            PaymentMode = v.PaymentMode,
            BankAccountId = v.BankAccountId,
            BankAccountName = v.BankAccount != null ? $"{v.BankAccount.AccountCode} - {v.BankAccount.AccountName}" : null,
            ReferenceNumber = v.ReferenceNumber,
            Narration = v.Narration,
            ExpenseVoucherId = v.ExpenseVoucherId,
            ExpenseVoucherNumber = v.ExpenseVoucher?.VoucherNumber,
            JournalEntryId = v.JournalEntryId,
            CreatedAt = v.CreatedAt,
            CreatedBy = v.CreatedBy
        };
    }
}
