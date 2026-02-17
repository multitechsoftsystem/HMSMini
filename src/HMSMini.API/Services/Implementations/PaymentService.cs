using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Payment;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IVoucherService _voucherService;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        ApplicationDbContext context,
        IVoucherService voucherService,
        IJournalEntryService journalEntryService,
        IChartOfAccountService chartOfAccountService,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _voucherService = voucherService;
        _journalEntryService = journalEntryService;
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        // Validate source exists
        int? companyId = null;
        string? roomNumber = null;

        if (dto.SourceType == PaymentSourceType.Room)
        {
            var checkIn = await _context.CheckIns
                .Include(c => c.Room)
                .FirstOrDefaultAsync(c => c.Id == dto.CheckInId && c.DeletedAt == null);

            if (checkIn == null)
                throw new NotFoundException(nameof(CheckIn), dto.CheckInId!);

            companyId = checkIn.CompanyId;
            roomNumber = checkIn.Room?.RoomNumber;
        }
        else if (dto.SourceType == PaymentSourceType.Banquet)
        {
            var booking = await _context.BanquetBookings
                .FirstOrDefaultAsync(b => b.Id == dto.BanquetBookingId && b.DeletedAt == null);

            if (booking == null)
                throw new NotFoundException(nameof(BanquetBooking), dto.BanquetBookingId!);

            if (booking.Status == BanquetBookingStatus.Cancelled)
                throw new BusinessRuleException("Cannot record payment for a cancelled booking");

            companyId = booking.CompanyId;
        }

        // Generate receipt number
        var receiptNumber = await GenerateReceiptNumberAsync();

        // Create payment
        var payment = new Payment
        {
            ReceiptNumber = receiptNumber,
            SourceType = dto.SourceType,
            CheckInId = dto.SourceType == PaymentSourceType.Room ? dto.CheckInId : null,
            BanquetBookingId = dto.SourceType == PaymentSourceType.Banquet ? dto.BanquetBookingId : null,
            CompanyId = companyId,
            PaymentDate = dto.PaymentDate,
            PaymentType = dto.PaymentType,
            PaymentMode = dto.PaymentMode,
            Amount = dto.Amount,
            ReferenceNumber = dto.ReferenceNumber,
            ReceivedBy = dto.ReceivedBy,
            Remarks = dto.Remarks
        };

        _context.Payments.Add(payment);

        // Post voucher for accounting (room payments only, non-refund)
        if (dto.SourceType == PaymentSourceType.Room
            && dto.CheckInId.HasValue
            && dto.PaymentType != PaymentType.Refund)
        {
            var voucherDto = new Models.DTOs.Voucher.CreateVoucherDto
            {
                VoucherType = VoucherType.Payment,
                Description = $"Payment {receiptNumber} - {dto.PaymentType} ({dto.PaymentMode})",
                Amount = dto.Amount,
                CheckInId = dto.CheckInId.Value,
                RoomNumber = roomNumber ?? "N/A",
                PostedBy = dto.ReceivedBy,
                AutoPostDaily = false
            };

            var voucher = await _voucherService.CreateVoucherAsync(voucherDto);
            payment.VoucherId = voucher.Id;
        }

        // Update Invoice TotalPaid/BalanceDue for room payments
        if (dto.SourceType == PaymentSourceType.Room && dto.CheckInId.HasValue)
        {
            await UpdateInvoicePaymentTotalsAsync(dto.CheckInId.Value);
        }

        // Update BanquetInvoice TotalPaid/BalanceDue for banquet payments
        if (dto.SourceType == PaymentSourceType.Banquet && dto.BanquetBookingId.HasValue)
        {
            await UpdateBanquetInvoicePaymentTotalsAsync(dto.BanquetBookingId.Value);
        }

        await _context.SaveChangesAsync();

        // Post journal entry: Dr. Cash/Bank / Cr. Accounts Receivable
        if (dto.PaymentType != PaymentType.Refund)
        {
            try
            {
                int debitAccountId;
                if (dto.PaymentMode == PaymentMode.Cash)
                    debitAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1001");
                else
                    debitAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1002");

                int creditAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1003"); // AR

                var lines = new List<(int accountId, decimal debit, decimal credit, string? desc)>
                {
                    (debitAccountId, dto.Amount, 0, $"Guest payment {receiptNumber}"),
                    (creditAccountId, 0, dto.Amount, $"AR cleared - {receiptNumber}")
                };

                await _journalEntryService.PostJournalEntryAsync(
                    dto.PaymentDate,
                    $"Guest Payment {receiptNumber}: {dto.SourceType} {dto.PaymentMode}",
                    JournalSourceType.GuestPayment,
                    payment.Id,
                    lines,
                    dto.ReceivedBy);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create journal entry for payment {ReceiptNumber}", receiptNumber);
            }
        }

        _logger.LogInformation("Payment {ReceiptNumber} recorded: {SourceType} Amount={Amount}",
            receiptNumber, dto.SourceType, dto.Amount);

        return (await GetByIdAsync(payment.Id))!;
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<List<PaymentDto>> GetByCheckInIdAsync(int checkInId)
    {
        var payments = await _context.Payments
            .Include(p => p.Company)
            .Where(p => p.CheckInId == checkInId)
            .OrderBy(p => p.PaymentDate)
            .AsNoTracking()
            .ToListAsync();

        return payments.Select(MapToDto).ToList();
    }

    public async Task<List<PaymentDto>> GetByBanquetBookingIdAsync(int bookingId)
    {
        var payments = await _context.Payments
            .Include(p => p.Company)
            .Where(p => p.BanquetBookingId == bookingId)
            .OrderBy(p => p.PaymentDate)
            .AsNoTracking()
            .ToListAsync();

        return payments.Select(MapToDto).ToList();
    }

    public async Task<PaymentSummaryDto> GetPaymentSummaryForCheckInAsync(int checkInId)
    {
        var checkIn = await _context.CheckIns.FindAsync(checkInId);
        if (checkIn == null || checkIn.DeletedAt != null)
            throw new NotFoundException(nameof(CheckIn), checkInId);

        var payments = await GetByCheckInIdAsync(checkInId);
        var totalPaid = payments.Sum(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        // Get total charged from invoice or vouchers
        decimal totalCharged = 0;
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.CheckInId == checkInId && i.DeletedAt == null);

        if (invoice != null)
        {
            totalCharged = invoice.GrandTotal;
        }
        else
        {
            // Sum from posted vouchers for active check-ins
            totalCharged = await _context.Vouchers
                .Where(v => v.CheckInId == checkInId && v.PostingStatus == "Posted")
                .SumAsync(v => v.Amount);
        }

        return new PaymentSummaryDto
        {
            TotalCharged = totalCharged,
            TotalPaid = totalPaid,
            BalanceDue = totalCharged - totalPaid,
            PaymentCount = payments.Count,
            Payments = payments
        };
    }

    public async Task<PaymentSummaryDto> GetPaymentSummaryForBanquetAsync(int bookingId)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        var payments = await GetByBanquetBookingIdAsync(bookingId);
        var totalPaid = payments.Sum(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        // Calculate total from banquet invoice or booking components
        decimal totalCharged = 0;
        var banquetInvoice = await _context.BanquetInvoices
            .FirstOrDefaultAsync(i => i.BanquetBookingId == bookingId && i.DeletedAt == null);

        if (banquetInvoice != null)
        {
            totalCharged = banquetInvoice.GrandTotal;
        }
        else
        {
            // Estimate from booking components
            var menuTotal = await _context.BanquetBookingMenus
                .Where(m => m.BanquetBookingId == bookingId && m.DeletedAt == null)
                .SumAsync(m => m.TotalAmount);

            var serviceTotal = await _context.BanquetBookingServices
                .Where(s => s.BanquetBookingId == bookingId && s.DeletedAt == null)
                .SumAsync(s => s.TotalAmount);

            var chargeTotal = await _context.BanquetCharges
                .Where(c => c.BanquetBookingId == bookingId && c.DeletedAt == null)
                .SumAsync(c => c.TotalAmount);

            totalCharged = booking.HallRent + menuTotal + serviceTotal + chargeTotal;
        }

        return new PaymentSummaryDto
        {
            TotalCharged = totalCharged,
            TotalPaid = totalPaid,
            BalanceDue = totalCharged - totalPaid,
            PaymentCount = payments.Count,
            Payments = payments
        };
    }

    public async Task<List<PaymentDto>> GetByCompanyIdAsync(int companyId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Payments
            .Include(p => p.Company)
            .Where(p => p.CompanyId == companyId);

        if (fromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(p => p.PaymentDate <= toDate.Value.Date);

        var payments = await query
            .OrderBy(p => p.PaymentDate)
            .AsNoTracking()
            .ToListAsync();

        return payments.Select(MapToDto).ToList();
    }

    public async Task<PaymentDto> CancelPaymentAsync(int id, string? reason, string? cancelledBy = null)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            throw new NotFoundException(nameof(Payment), id);

        if (payment.DeletedAt != null)
            throw new BusinessRuleException("Payment is already cancelled");

        // Soft delete the payment
        payment.DeletedAt = DateTime.UtcNow;
        payment.DeletedBy = cancelledBy;

        // If there's a linked voucher, cancel it
        if (payment.VoucherId.HasValue)
        {
            await _voucherService.CancelVoucherAsync(payment.VoucherId.Value,
                new Models.DTOs.Voucher.CancelVoucherDto
                {
                    CancelledBy = cancelledBy,
                    CancellationReason = reason ?? "Payment cancelled"
                });
        }

        // Update invoice totals
        if (payment.SourceType == PaymentSourceType.Room && payment.CheckInId.HasValue)
        {
            await UpdateInvoicePaymentTotalsAsync(payment.CheckInId.Value);
        }
        else if (payment.SourceType == PaymentSourceType.Banquet && payment.BanquetBookingId.HasValue)
        {
            await UpdateBanquetInvoicePaymentTotalsAsync(payment.BanquetBookingId.Value);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {ReceiptNumber} cancelled by {User}. Reason: {Reason}",
            payment.ReceiptNumber, cancelledBy ?? "System", reason);

        return MapToDto(payment);
    }

    private async Task UpdateInvoicePaymentTotalsAsync(int checkInId)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.CheckInId == checkInId && i.DeletedAt == null);

        if (invoice == null)
            return;

        var totalPaid = await _context.Payments
            .Where(p => p.CheckInId == checkInId && p.DeletedAt == null)
            .SumAsync(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        invoice.TotalPaid = totalPaid;
        invoice.BalanceDue = invoice.GrandTotal - totalPaid;
        invoice.PaymentStatus = totalPaid >= invoice.GrandTotal ? "Paid"
            : totalPaid > 0 ? "PartiallyPaid"
            : "Unpaid";
    }

    private async Task UpdateBanquetInvoicePaymentTotalsAsync(int bookingId)
    {
        var invoice = await _context.BanquetInvoices
            .FirstOrDefaultAsync(i => i.BanquetBookingId == bookingId && i.DeletedAt == null);

        if (invoice == null)
            return;

        var totalPaid = await _context.Payments
            .Where(p => p.BanquetBookingId == bookingId && p.DeletedAt == null)
            .SumAsync(p => p.PaymentType == PaymentType.Refund ? -p.Amount : p.Amount);

        invoice.TotalPaid = totalPaid;
        invoice.BalanceDue = invoice.GrandTotal - totalPaid;
        invoice.PaymentStatus = totalPaid >= invoice.GrandTotal ? "Paid"
            : totalPaid > 0 ? "PartiallyPaid"
            : "Unpaid";
    }

    private async Task<string> GenerateReceiptNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"RCT-{year}-";

        var lastPayment = await _context.Payments
            .IgnoreQueryFilters()
            .Where(p => p.ReceiptNumber.StartsWith(prefix))
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastPayment != null)
        {
            var lastNumberStr = lastPayment.ReceiptNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D5}";
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            ReceiptNumber = payment.ReceiptNumber,
            SourceType = payment.SourceType,
            CheckInId = payment.CheckInId,
            BanquetBookingId = payment.BanquetBookingId,
            CompanyId = payment.CompanyId,
            CompanyName = payment.Company?.CompanyName,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            PaymentMode = payment.PaymentMode,
            Amount = payment.Amount,
            ReferenceNumber = payment.ReferenceNumber,
            ReceivedBy = payment.ReceivedBy,
            Remarks = payment.Remarks,
            VoucherId = payment.VoucherId,
            CreatedAt = payment.CreatedAt,
            CreatedBy = payment.CreatedBy
        };
    }
}
