using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BanquetPayment;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

[Obsolete("Use PaymentService instead. This service will be removed in a future version.")]
public class BanquetPaymentService : IBanquetPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BanquetPaymentService> _logger;

    public BanquetPaymentService(ApplicationDbContext context, ILogger<BanquetPaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BanquetPaymentDto>> GetByBookingAsync(int bookingId)
    {
        return await _context.BanquetPayments
            .Where(p => p.BanquetBookingId == bookingId && p.DeletedAt == null)
            .Select(p => new BanquetPaymentDto
            {
                Id = p.Id,
                BanquetBookingId = p.BanquetBookingId,
                ReceiptNumber = p.ReceiptNumber,
                PaymentDate = p.PaymentDate,
                PaymentType = p.PaymentType,
                PaymentMode = p.PaymentMode,
                Amount = p.Amount,
                ReferenceNumber = p.ReferenceNumber,
                ReceivedBy = p.ReceivedBy,
                CreatedAt = p.CreatedAt
            })
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<BanquetPaymentDto> CreateAsync(int bookingId, CreateBanquetPaymentDto dto)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        if (booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot record payment for a cancelled booking");

        var receiptNumber = await GenerateReceiptNumberAsync();

        var payment = new BanquetPayment
        {
            BanquetBookingId = bookingId,
            ReceiptNumber = receiptNumber,
            PaymentDate = dto.PaymentDate,
            PaymentType = dto.PaymentType,
            PaymentMode = dto.PaymentMode,
            Amount = dto.Amount,
            ReferenceNumber = dto.ReferenceNumber,
            ReceivedBy = dto.ReceivedBy
        };

        _context.BanquetPayments.Add(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banquet payment {ReceiptNumber} recorded for booking {BookingId}: {Amount}",
            receiptNumber, bookingId, dto.Amount);

        return new BanquetPaymentDto
        {
            Id = payment.Id,
            BanquetBookingId = payment.BanquetBookingId,
            ReceiptNumber = payment.ReceiptNumber,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            PaymentMode = payment.PaymentMode,
            Amount = payment.Amount,
            ReferenceNumber = payment.ReferenceNumber,
            ReceivedBy = payment.ReceivedBy,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<BanquetPaymentSummaryDto> GetPaymentSummaryAsync(int bookingId)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        var payments = await GetByBookingAsync(bookingId);
        var totalPaid = payments.Sum(p => p.Amount);

        // Calculate total amount from booking (simple estimate - hall rent + menus + services + charges)
        var menuTotal = await _context.BanquetBookingMenus
            .Where(m => m.BanquetBookingId == bookingId && m.DeletedAt == null)
            .SumAsync(m => m.TotalAmount);

        var serviceTotal = await _context.BanquetBookingServices
            .Where(s => s.BanquetBookingId == bookingId && s.DeletedAt == null)
            .SumAsync(s => s.TotalAmount);

        var chargeTotal = await _context.BanquetCharges
            .Where(c => c.BanquetBookingId == bookingId && c.DeletedAt == null)
            .SumAsync(c => c.TotalAmount);

        var totalAmount = booking.HallRent + menuTotal + serviceTotal + chargeTotal;

        return new BanquetPaymentSummaryDto
        {
            BanquetBookingId = bookingId,
            TotalAmount = totalAmount,
            TotalPaid = totalPaid,
            BalanceDue = totalAmount - totalPaid,
            PaymentCount = payments.Count,
            Payments = payments
        };
    }

    private async Task<string> GenerateReceiptNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"BPY-{year}-";

        var lastPayment = await _context.BanquetPayments
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
}
