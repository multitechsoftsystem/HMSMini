using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BanquetCharge;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class BanquetChargeService : IBanquetChargeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BanquetChargeService> _logger;

    public BanquetChargeService(ApplicationDbContext context, ILogger<BanquetChargeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BanquetChargeDto>> GetByBookingAsync(int bookingId)
    {
        return await _context.BanquetCharges
            .Where(c => c.BanquetBookingId == bookingId && c.DeletedAt == null)
            .Select(c => new BanquetChargeDto
            {
                Id = c.Id,
                BanquetBookingId = c.BanquetBookingId,
                ChargeDate = c.ChargeDate,
                ChargeType = c.ChargeType,
                Description = c.Description,
                Amount = c.Amount,
                Quantity = c.Quantity,
                TotalAmount = c.TotalAmount,
                ApplyTax = c.ApplyTax,
                VoucherTaxConfigId = c.VoucherTaxConfigId,
                CreatedAt = c.CreatedAt
            })
            .OrderBy(c => c.ChargeDate)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<BanquetChargeDto> CreateAsync(int bookingId, CreateBanquetChargeDto dto)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        if (booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot add charges to a cancelled booking");

        var charge = new BanquetCharge
        {
            BanquetBookingId = bookingId,
            ChargeDate = dto.ChargeDate,
            ChargeType = dto.ChargeType,
            Description = dto.Description,
            Amount = dto.Amount,
            Quantity = dto.Quantity,
            ApplyTax = dto.ApplyTax,
            VoucherTaxConfigId = dto.VoucherTaxConfigId
        };

        _context.BanquetCharges.Add(charge);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banquet charge added to booking {BookingId}: {ChargeType} - {Amount}",
            bookingId, dto.ChargeType, dto.Amount);

        return new BanquetChargeDto
        {
            Id = charge.Id,
            BanquetBookingId = charge.BanquetBookingId,
            ChargeDate = charge.ChargeDate,
            ChargeType = charge.ChargeType,
            Description = charge.Description,
            Amount = charge.Amount,
            Quantity = charge.Quantity,
            TotalAmount = charge.Amount * charge.Quantity,
            ApplyTax = charge.ApplyTax,
            VoucherTaxConfigId = charge.VoucherTaxConfigId,
            CreatedAt = charge.CreatedAt
        };
    }

    public async Task<BanquetChargeDto> UpdateAsync(int chargeId, UpdateBanquetChargeDto dto)
    {
        var charge = await _context.BanquetCharges
            .Include(c => c.BanquetBooking)
            .FirstOrDefaultAsync(c => c.Id == chargeId && c.DeletedAt == null);

        if (charge == null)
            throw new NotFoundException(nameof(BanquetCharge), chargeId);

        if (charge.BanquetBooking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot update charges for a cancelled booking");

        charge.ChargeDate = dto.ChargeDate;
        charge.ChargeType = dto.ChargeType;
        charge.Description = dto.Description;
        charge.Amount = dto.Amount;
        charge.Quantity = dto.Quantity;
        charge.ApplyTax = dto.ApplyTax;
        charge.VoucherTaxConfigId = dto.VoucherTaxConfigId;

        await _context.SaveChangesAsync();

        return new BanquetChargeDto
        {
            Id = charge.Id,
            BanquetBookingId = charge.BanquetBookingId,
            ChargeDate = charge.ChargeDate,
            ChargeType = charge.ChargeType,
            Description = charge.Description,
            Amount = charge.Amount,
            Quantity = charge.Quantity,
            TotalAmount = charge.TotalAmount,
            ApplyTax = charge.ApplyTax,
            VoucherTaxConfigId = charge.VoucherTaxConfigId,
            CreatedAt = charge.CreatedAt
        };
    }

    public async Task DeleteAsync(int chargeId)
    {
        var charge = await _context.BanquetCharges
            .Include(c => c.BanquetBooking)
            .FirstOrDefaultAsync(c => c.Id == chargeId && c.DeletedAt == null);

        if (charge == null)
            throw new NotFoundException(nameof(BanquetCharge), chargeId);

        charge.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
