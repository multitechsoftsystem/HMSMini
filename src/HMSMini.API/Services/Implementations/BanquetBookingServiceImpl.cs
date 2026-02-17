using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BanquetBooking;
using HMSMini.API.Models.DTOs.BanquetBookingMenu;
using HMSMini.API.Models.DTOs.BanquetBookingService;
using HMSMini.API.Models.DTOs.BanquetCharge;
using HMSMini.API.Models.DTOs.BanquetPayment;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class BanquetBookingServiceImpl : IBanquetBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IBanquetHallService _hallService;
    private readonly ITaxService _taxService;
    private readonly ILogger<BanquetBookingServiceImpl> _logger;

    public BanquetBookingServiceImpl(
        ApplicationDbContext context,
        IBanquetHallService hallService,
        ITaxService taxService,
        ILogger<BanquetBookingServiceImpl> logger)
    {
        _context = context;
        _hallService = hallService;
        _taxService = taxService;
        _logger = logger;
    }

    public async Task<List<BanquetBookingListDto>> GetAllAsync()
    {
        return await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .Where(b => b.DeletedAt == null)
            .Select(b => new BanquetBookingListDto
            {
                Id = b.Id,
                BookingNumber = b.BookingNumber,
                HallName = b.BanquetHall.HallName,
                EventTypeName = b.EventType.EventTypeName,
                EventDate = b.EventDate,
                EventStartTime = b.EventStartTime,
                EventEndTime = b.EventEndTime,
                ExpectedGuests = b.ExpectedGuests,
                Status = b.Status,
                ContactPersonName = b.ContactPersonName,
                ContactPhone = b.ContactPhone,
                CompanyName = b.Company != null ? b.Company.CompanyName : null,
                HallRent = b.HallRent
            })
            .OrderByDescending(b => b.EventDate)
            .ThenBy(b => b.EventStartTime)
            .ToListAsync();
    }

    public async Task<BanquetBookingDetailDto?> GetByIdAsync(int id)
    {
        var booking = await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (booking == null) return null;

        var menus = await GetMenusByBookingAsync(id);
        var services = await GetServicesByBookingAsync(id);
        var charges = await _context.BanquetCharges
            .Where(c => c.BanquetBookingId == id && c.DeletedAt == null)
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
            }).OrderBy(c => c.ChargeDate).ToListAsync();

        var payments = await _context.BanquetPayments
            .Where(p => p.BanquetBookingId == id && p.DeletedAt == null)
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
            }).OrderBy(p => p.PaymentDate).ToListAsync();

        return new BanquetBookingDetailDto
        {
            Id = booking.Id,
            BookingNumber = booking.BookingNumber,
            BanquetHallId = booking.BanquetHallId,
            HallName = booking.BanquetHall.HallName,
            EventTypeId = booking.EventTypeId,
            EventTypeName = booking.EventType.EventTypeName,
            EventDate = booking.EventDate,
            EventStartTime = booking.EventStartTime,
            EventEndTime = booking.EventEndTime,
            ExpectedGuests = booking.ExpectedGuests,
            ActualGuests = booking.ActualGuests,
            Status = booking.Status,
            PricingType = booking.PricingType,
            ContactPersonName = booking.ContactPersonName,
            ContactPhone = booking.ContactPhone,
            CompanyId = booking.CompanyId,
            CompanyName = booking.Company?.CompanyName,
            CheckInId = booking.CheckInId,
            TaxType = booking.TaxType,
            DiscountPercentage = booking.DiscountPercentage,
            HallRent = booking.HallRent,
            Remarks = booking.Remarks,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            Menus = menus,
            Services = services,
            Charges = charges,
            Payments = payments
        };
    }

    public async Task<BanquetBookingDto> CreateAsync(CreateBanquetBookingDto dto)
    {
        // Check hall availability
        var isAvailable = await _hallService.CheckAvailabilityAsync(
            dto.BanquetHallId, dto.EventDate, dto.EventStartTime, dto.EventEndTime);
        if (!isAvailable)
            throw new BusinessRuleException("Hall is not available for the selected date and time");

        // Generate booking number
        var bookingNumber = await GenerateBookingNumberAsync();

        // Capture tax snapshot
        string? taxSnapshotJson = null;
        try
        {
            var snapshot = await _taxService.CreateTaxSlabSnapshotAsync(dto.EventDate);
            taxSnapshotJson = System.Text.Json.JsonSerializer.Serialize(snapshot);
        }
        catch { /* Tax snapshot is optional */ }

        var booking = new BanquetBooking
        {
            BookingNumber = bookingNumber,
            BanquetHallId = dto.BanquetHallId,
            EventTypeId = dto.EventTypeId,
            EventDate = dto.EventDate,
            EventStartTime = dto.EventStartTime,
            EventEndTime = dto.EventEndTime,
            ExpectedGuests = dto.ExpectedGuests,
            Status = BanquetBookingStatus.Enquiry,
            PricingType = dto.PricingType,
            ContactPersonName = dto.ContactPersonName,
            ContactPhone = dto.ContactPhone,
            CompanyId = dto.CompanyId,
            CheckInId = dto.CheckInId,
            TaxType = dto.TaxType,
            TaxSlabSnapshotJson = taxSnapshotJson,
            DiscountPercentage = dto.DiscountPercentage,
            HallRent = dto.HallRent,
            Remarks = dto.Remarks
        };

        _context.BanquetBookings.Add(booking);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banquet booking {BookingNumber} created", bookingNumber);

        return await MapToDto(booking);
    }

    public async Task<BanquetBookingDto> UpdateAsync(int id, UpdateBanquetBookingDto dto)
    {
        var booking = await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            throw new NotFoundException(nameof(BanquetBooking), id);

        if (booking.Status == BanquetBookingStatus.Completed || booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot update a completed or cancelled booking");

        // Check hall availability if hall or time changed
        if (booking.BanquetHallId != dto.BanquetHallId ||
            booking.EventDate != dto.EventDate ||
            booking.EventStartTime != dto.EventStartTime ||
            booking.EventEndTime != dto.EventEndTime)
        {
            var isAvailable = await _hallService.CheckAvailabilityAsync(
                dto.BanquetHallId, dto.EventDate, dto.EventStartTime, dto.EventEndTime, id);
            if (!isAvailable)
                throw new BusinessRuleException("Hall is not available for the selected date and time");
        }

        booking.BanquetHallId = dto.BanquetHallId;
        booking.EventTypeId = dto.EventTypeId;
        booking.EventDate = dto.EventDate;
        booking.EventStartTime = dto.EventStartTime;
        booking.EventEndTime = dto.EventEndTime;
        booking.ExpectedGuests = dto.ExpectedGuests;
        booking.ActualGuests = dto.ActualGuests;
        booking.PricingType = dto.PricingType;
        booking.ContactPersonName = dto.ContactPersonName;
        booking.ContactPhone = dto.ContactPhone;
        booking.CompanyId = dto.CompanyId;
        booking.CheckInId = dto.CheckInId;
        booking.TaxType = dto.TaxType;
        booking.DiscountPercentage = dto.DiscountPercentage;
        booking.HallRent = dto.HallRent;
        booking.Remarks = dto.Remarks;

        await _context.SaveChangesAsync();
        return await MapToDto(booking);
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await _context.BanquetBookings.FindAsync(id);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), id);

        booking.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<BanquetBookingDto> UpdateStatusAsync(int id, UpdateBanquetBookingStatusDto dto)
    {
        var booking = await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            throw new NotFoundException(nameof(BanquetBooking), id);

        // Validate status transitions
        ValidateStatusTransition(booking.Status, dto.NewStatus);

        booking.Status = dto.NewStatus;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banquet booking {BookingNumber} status changed to {Status}",
            booking.BookingNumber, dto.NewStatus);

        return await MapToDto(booking);
    }

    // === Menu Management ===

    public async Task<List<BanquetBookingMenuDto>> GetMenusByBookingAsync(int bookingId)
    {
        return await _context.BanquetBookingMenus
            .Include(m => m.MenuPackage)
            .Include(m => m.MenuItem)
            .Where(m => m.BanquetBookingId == bookingId && m.DeletedAt == null)
            .Select(m => new BanquetBookingMenuDto
            {
                Id = m.Id,
                BanquetBookingId = m.BanquetBookingId,
                MenuPackageId = m.MenuPackageId,
                PackageName = m.MenuPackage != null ? m.MenuPackage.PackageName : null,
                MenuItemId = m.MenuItemId,
                ItemName = m.ItemName ?? (m.MenuItem != null ? m.MenuItem.ItemName : null),
                MenuDate = m.MenuDate,
                Quantity = m.Quantity,
                RatePerPlate = m.RatePerPlate,
                TotalAmount = m.TotalAmount,
                ApplyTax = m.ApplyTax,
                VoucherTaxConfigId = m.VoucherTaxConfigId,
                CreatedAt = m.CreatedAt
            }).ToListAsync();
    }

    public async Task<BanquetBookingMenuDto> AddMenuAsync(int bookingId, CreateBanquetBookingMenuDto dto)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        if (booking.Status == BanquetBookingStatus.Completed || booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot modify menus for a completed or cancelled booking");

        var menu = new BanquetBookingMenu
        {
            BanquetBookingId = bookingId,
            MenuPackageId = dto.MenuPackageId,
            MenuItemId = dto.MenuItemId,
            ItemName = dto.ItemName,
            MenuDate = dto.MenuDate,
            Quantity = dto.Quantity,
            RatePerPlate = dto.RatePerPlate,
            ApplyTax = dto.ApplyTax,
            VoucherTaxConfigId = dto.VoucherTaxConfigId
        };

        _context.BanquetBookingMenus.Add(menu);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(menu).Reference(m => m.MenuPackage).LoadAsync();
        await _context.Entry(menu).Reference(m => m.MenuItem).LoadAsync();

        return new BanquetBookingMenuDto
        {
            Id = menu.Id,
            BanquetBookingId = menu.BanquetBookingId,
            MenuPackageId = menu.MenuPackageId,
            PackageName = menu.MenuPackage?.PackageName,
            MenuItemId = menu.MenuItemId,
            ItemName = menu.ItemName ?? menu.MenuItem?.ItemName,
            MenuDate = menu.MenuDate,
            Quantity = menu.Quantity,
            RatePerPlate = menu.RatePerPlate,
            TotalAmount = menu.RatePerPlate * menu.Quantity,
            ApplyTax = menu.ApplyTax,
            VoucherTaxConfigId = menu.VoucherTaxConfigId,
            CreatedAt = menu.CreatedAt
        };
    }

    public async Task<BanquetBookingMenuDto> UpdateMenuAsync(int menuId, UpdateBanquetBookingMenuDto dto)
    {
        var menu = await _context.BanquetBookingMenus
            .Include(m => m.BanquetBooking)
            .Include(m => m.MenuPackage)
            .Include(m => m.MenuItem)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.DeletedAt == null);

        if (menu == null)
            throw new NotFoundException(nameof(BanquetBookingMenu), menuId);

        if (menu.BanquetBooking.Status == BanquetBookingStatus.Completed || menu.BanquetBooking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot modify menus for a completed or cancelled booking");

        menu.Quantity = dto.Quantity;
        menu.RatePerPlate = dto.RatePerPlate;

        await _context.SaveChangesAsync();

        return new BanquetBookingMenuDto
        {
            Id = menu.Id,
            BanquetBookingId = menu.BanquetBookingId,
            MenuPackageId = menu.MenuPackageId,
            PackageName = menu.MenuPackage?.PackageName,
            MenuItemId = menu.MenuItemId,
            ItemName = menu.ItemName ?? menu.MenuItem?.ItemName,
            MenuDate = menu.MenuDate,
            Quantity = menu.Quantity,
            RatePerPlate = menu.RatePerPlate,
            TotalAmount = menu.TotalAmount,
            ApplyTax = menu.ApplyTax,
            VoucherTaxConfigId = menu.VoucherTaxConfigId,
            CreatedAt = menu.CreatedAt
        };
    }

    public async Task DeleteMenuAsync(int menuId)
    {
        var menu = await _context.BanquetBookingMenus
            .Include(m => m.BanquetBooking)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.DeletedAt == null);

        if (menu == null)
            throw new NotFoundException(nameof(BanquetBookingMenu), menuId);

        menu.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // === Service Management ===

    public async Task<List<BanquetBookingServiceDto>> GetServicesByBookingAsync(int bookingId)
    {
        return await _context.BanquetBookingServices
            .Where(s => s.BanquetBookingId == bookingId && s.DeletedAt == null)
            .Select(s => new BanquetBookingServiceDto
            {
                Id = s.Id,
                BanquetBookingId = s.BanquetBookingId,
                BanquetServiceId = s.BanquetServiceId,
                ServiceName = s.ServiceName,
                ServiceDate = s.ServiceDate,
                Quantity = s.Quantity,
                Rate = s.Rate,
                TotalAmount = s.TotalAmount,
                ApplyTax = s.ApplyTax,
                VoucherTaxConfigId = s.VoucherTaxConfigId,
                CreatedAt = s.CreatedAt
            }).ToListAsync();
    }

    public async Task<BanquetBookingServiceDto> AddServiceAsync(int bookingId, CreateBanquetBookingServiceDto dto)
    {
        var booking = await _context.BanquetBookings.FindAsync(bookingId);
        if (booking == null || booking.DeletedAt != null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        if (booking.Status == BanquetBookingStatus.Completed || booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot modify services for a completed or cancelled booking");

        var service = new Models.Entities.BanquetBookingService
        {
            BanquetBookingId = bookingId,
            BanquetServiceId = dto.BanquetServiceId,
            ServiceName = dto.ServiceName,
            ServiceDate = dto.ServiceDate,
            Quantity = dto.Quantity,
            Rate = dto.Rate,
            ApplyTax = dto.ApplyTax,
            VoucherTaxConfigId = dto.VoucherTaxConfigId
        };

        _context.BanquetBookingServices.Add(service);
        await _context.SaveChangesAsync();

        return new BanquetBookingServiceDto
        {
            Id = service.Id,
            BanquetBookingId = service.BanquetBookingId,
            BanquetServiceId = service.BanquetServiceId,
            ServiceName = service.ServiceName,
            ServiceDate = service.ServiceDate,
            Quantity = service.Quantity,
            Rate = service.Rate,
            TotalAmount = service.Rate * service.Quantity,
            ApplyTax = service.ApplyTax,
            VoucherTaxConfigId = service.VoucherTaxConfigId,
            CreatedAt = service.CreatedAt
        };
    }

    public async Task<BanquetBookingServiceDto> UpdateServiceAsync(int serviceId, UpdateBanquetBookingServiceDto dto)
    {
        var service = await _context.BanquetBookingServices
            .Include(s => s.BanquetBooking)
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.DeletedAt == null);

        if (service == null)
            throw new NotFoundException(nameof(Models.Entities.BanquetBookingService), serviceId);

        if (service.BanquetBooking.Status == BanquetBookingStatus.Completed || service.BanquetBooking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot modify services for a completed or cancelled booking");

        service.ServiceName = dto.ServiceName;
        service.Quantity = dto.Quantity;
        service.Rate = dto.Rate;
        service.ApplyTax = dto.ApplyTax;
        service.VoucherTaxConfigId = dto.VoucherTaxConfigId;

        await _context.SaveChangesAsync();

        return new BanquetBookingServiceDto
        {
            Id = service.Id,
            BanquetBookingId = service.BanquetBookingId,
            BanquetServiceId = service.BanquetServiceId,
            ServiceName = service.ServiceName,
            ServiceDate = service.ServiceDate,
            Quantity = service.Quantity,
            Rate = service.Rate,
            TotalAmount = service.TotalAmount,
            ApplyTax = service.ApplyTax,
            VoucherTaxConfigId = service.VoucherTaxConfigId,
            CreatedAt = service.CreatedAt
        };
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        var service = await _context.BanquetBookingServices
            .Include(s => s.BanquetBooking)
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.DeletedAt == null);

        if (service == null)
            throw new NotFoundException(nameof(Models.Entities.BanquetBookingService), serviceId);

        service.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // === Private Helpers ===

    private void ValidateStatusTransition(BanquetBookingStatus current, BanquetBookingStatus newStatus)
    {
        var validTransitions = new Dictionary<BanquetBookingStatus, BanquetBookingStatus[]>
        {
            { BanquetBookingStatus.Enquiry, new[] { BanquetBookingStatus.Confirmed, BanquetBookingStatus.Cancelled } },
            { BanquetBookingStatus.Confirmed, new[] { BanquetBookingStatus.InProgress, BanquetBookingStatus.Cancelled } },
            { BanquetBookingStatus.InProgress, new[] { BanquetBookingStatus.Completed } },
            { BanquetBookingStatus.Completed, Array.Empty<BanquetBookingStatus>() },
            { BanquetBookingStatus.Cancelled, Array.Empty<BanquetBookingStatus>() }
        };

        if (!validTransitions.ContainsKey(current) || !validTransitions[current].Contains(newStatus))
        {
            throw new BusinessRuleException(
                $"Cannot transition from {current} to {newStatus}");
        }
    }

    private async Task<string> GenerateBookingNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"BNQ-{year}-";

        var lastBooking = await _context.BanquetBookings
            .Where(b => b.BookingNumber.StartsWith(prefix))
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastBooking != null)
        {
            var lastNumberStr = lastBooking.BookingNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D5}";
    }

    private async Task<BanquetBookingDto> MapToDto(BanquetBooking booking)
    {
        // Ensure navigation properties are loaded
        if (booking.BanquetHall == null)
            await _context.Entry(booking).Reference(b => b.BanquetHall).LoadAsync();
        if (booking.EventType == null)
            await _context.Entry(booking).Reference(b => b.EventType).LoadAsync();
        if (booking.Company == null && booking.CompanyId.HasValue)
            await _context.Entry(booking).Reference(b => b.Company).LoadAsync();

        return new BanquetBookingDto
        {
            Id = booking.Id,
            BookingNumber = booking.BookingNumber,
            BanquetHallId = booking.BanquetHallId,
            HallName = booking.BanquetHall.HallName,
            EventTypeId = booking.EventTypeId,
            EventTypeName = booking.EventType.EventTypeName,
            EventDate = booking.EventDate,
            EventStartTime = booking.EventStartTime,
            EventEndTime = booking.EventEndTime,
            ExpectedGuests = booking.ExpectedGuests,
            ActualGuests = booking.ActualGuests,
            Status = booking.Status,
            PricingType = booking.PricingType,
            ContactPersonName = booking.ContactPersonName,
            ContactPhone = booking.ContactPhone,
            CompanyId = booking.CompanyId,
            CompanyName = booking.Company?.CompanyName,
            CheckInId = booking.CheckInId,
            TaxType = booking.TaxType,
            DiscountPercentage = booking.DiscountPercentage,
            HallRent = booking.HallRent,
            Remarks = booking.Remarks,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}
