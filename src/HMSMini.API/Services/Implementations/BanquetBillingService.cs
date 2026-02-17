using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Models.DTOs.BanquetBilling;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class BanquetBillingService : IBanquetBillingService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaxService _taxService;
    private readonly ILogger<BanquetBillingService> _logger;

    public BanquetBillingService(
        ApplicationDbContext context,
        ITaxService taxService,
        ILogger<BanquetBillingService> logger)
    {
        _context = context;
        _taxService = taxService;
        _logger = logger;
    }

    public async Task<BanquetBillPreviewDto> GetBillPreviewAsync(int bookingId)
    {
        var booking = await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeletedAt == null);

        if (booking == null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        // Parse tax snapshot
        TaxSlabSnapshot? taxSnapshot = null;
        if (!string.IsNullOrEmpty(booking.TaxSlabSnapshotJson))
        {
            try
            {
                taxSnapshot = JsonSerializer.Deserialize<TaxSlabSnapshot>(booking.TaxSlabSnapshotJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize tax snapshot for booking {BookingId}", bookingId);
            }
        }

        // Get menu charges with VoucherTaxConfig
        var menus = await _context.BanquetBookingMenus
            .Include(m => m.MenuPackage)
            .Include(m => m.MenuItem)
            .Include(m => m.VoucherTaxConfig)
            .Where(m => m.BanquetBookingId == bookingId && m.DeletedAt == null)
            .ToListAsync();

        // Get service charges with VoucherTaxConfig
        var services = await _context.BanquetBookingServices
            .Include(s => s.VoucherTaxConfig)
            .Where(s => s.BanquetBookingId == bookingId && s.DeletedAt == null)
            .ToListAsync();

        // Get additional charges with VoucherTaxConfig
        var charges = await _context.BanquetCharges
            .Include(c => c.VoucherTaxConfig)
            .Where(c => c.BanquetBookingId == bookingId && c.DeletedAt == null)
            .ToListAsync();

        // Calculate subtotals
        var menuChargesSubtotal = menus.Sum(m => m.TotalAmount);
        var serviceChargesSubtotal = services.Sum(s => s.TotalAmount);
        var additionalChargesSubtotal = charges.Sum(c => c.TotalAmount);
        var subtotal = booking.HallRent + menuChargesSubtotal + serviceChargesSubtotal + additionalChargesSubtotal;

        // Apply discount
        var discountAmount = subtotal * booking.DiscountPercentage / 100;
        var discountMultiplier = subtotal > 0 ? (1 - booking.DiscountPercentage / 100) : 1;
        var subtotalAfterDiscount = subtotal - discountAmount;

        // Get fallback tax rates from TaxSlab
        var fallbackTaxLines = await _taxService.CalculateTaxAsync(
            1m, booking.TaxType, booking.EventDate, taxSnapshot);
        decimal fallbackCgst = 0, fallbackSgst = 0, fallbackIgst = 0;
        foreach (var tl in fallbackTaxLines)
        {
            if (tl.TaxType == "CGST") fallbackCgst = tl.TaxPercentage;
            else if (tl.TaxType == "SGST") fallbackSgst = tl.TaxPercentage;
            else if (tl.TaxType == "IGST") fallbackIgst = tl.TaxPercentage;
        }

        // Accumulate all tax lines
        var allTaxLines = new List<TaxSummaryDto>();

        // Hall rent tax (always uses fallback/TaxSlab)
        var hallRentAfterDiscount = booking.HallRent * discountMultiplier;
        CalculateVoucherTax(hallRentAfterDiscount, booking.TaxType, null,
            fallbackCgst, fallbackSgst, fallbackIgst, allTaxLines, out _);

        // Build menu charge DTOs with per-item tax
        var menuCharges = menus.Select(m =>
        {
            decimal itemTax = 0;
            decimal cgst = 0, sgst = 0, igst = 0;
            if (m.ApplyTax)
            {
                var amountAfterDiscount = m.TotalAmount * discountMultiplier;
                CalculateVoucherTax(amountAfterDiscount, booking.TaxType, m.VoucherTaxConfig,
                    fallbackCgst, fallbackSgst, fallbackIgst, allTaxLines, out itemTax);
                cgst = m.VoucherTaxConfig?.CgstPercentage ?? fallbackCgst;
                sgst = m.VoucherTaxConfig?.SgstPercentage ?? fallbackSgst;
                igst = m.VoucherTaxConfig?.IgstPercentage ?? fallbackIgst;
            }

            return new BanquetMenuChargeDto
            {
                PackageName = m.MenuPackage?.PackageName,
                ItemName = m.ItemName ?? m.MenuItem?.ItemName,
                Quantity = m.Quantity,
                RatePerPlate = m.RatePerPlate,
                TotalAmount = m.TotalAmount,
                ApplyTax = m.ApplyTax,
                VoucherTaxConfigId = m.VoucherTaxConfigId,
                TaxConfigName = m.VoucherTaxConfig?.VoucherType,
                SACCode = m.VoucherTaxConfig?.SACCode,
                CgstPercentage = cgst,
                SgstPercentage = sgst,
                IgstPercentage = igst,
                TaxAmount = itemTax
            };
        }).ToList();

        // Build service charge DTOs with per-item tax
        var serviceCharges = services.Select(s =>
        {
            decimal itemTax = 0;
            decimal cgst = 0, sgst = 0, igst = 0;
            if (s.ApplyTax)
            {
                var amountAfterDiscount = s.TotalAmount * discountMultiplier;
                CalculateVoucherTax(amountAfterDiscount, booking.TaxType, s.VoucherTaxConfig,
                    fallbackCgst, fallbackSgst, fallbackIgst, allTaxLines, out itemTax);
                cgst = s.VoucherTaxConfig?.CgstPercentage ?? fallbackCgst;
                sgst = s.VoucherTaxConfig?.SgstPercentage ?? fallbackSgst;
                igst = s.VoucherTaxConfig?.IgstPercentage ?? fallbackIgst;
            }

            return new BanquetServiceChargeDto
            {
                ServiceName = s.ServiceName,
                Quantity = s.Quantity,
                Rate = s.Rate,
                TotalAmount = s.TotalAmount,
                ApplyTax = s.ApplyTax,
                VoucherTaxConfigId = s.VoucherTaxConfigId,
                TaxConfigName = s.VoucherTaxConfig?.VoucherType,
                SACCode = s.VoucherTaxConfig?.SACCode,
                CgstPercentage = cgst,
                SgstPercentage = sgst,
                IgstPercentage = igst,
                TaxAmount = itemTax
            };
        }).ToList();

        // Build additional charge DTOs with per-item tax
        var additionalCharges = charges.Select(c =>
        {
            decimal itemTax = 0;
            decimal cgst = 0, sgst = 0, igst = 0;
            if (c.ApplyTax)
            {
                var amountAfterDiscount = c.TotalAmount * discountMultiplier;
                CalculateVoucherTax(amountAfterDiscount, booking.TaxType, c.VoucherTaxConfig,
                    fallbackCgst, fallbackSgst, fallbackIgst, allTaxLines, out itemTax);
                cgst = c.VoucherTaxConfig?.CgstPercentage ?? fallbackCgst;
                sgst = c.VoucherTaxConfig?.SgstPercentage ?? fallbackSgst;
                igst = c.VoucherTaxConfig?.IgstPercentage ?? fallbackIgst;
            }

            return new BanquetAdditionalChargeDto
            {
                ChargeType = c.ChargeType,
                Description = c.Description,
                Amount = c.Amount,
                Quantity = c.Quantity,
                TotalAmount = c.TotalAmount,
                ApplyTax = c.ApplyTax,
                VoucherTaxConfigId = c.VoucherTaxConfigId,
                TaxConfigName = c.VoucherTaxConfig?.VoucherType,
                SACCode = c.VoucherTaxConfig?.SACCode,
                CgstPercentage = cgst,
                SgstPercentage = sgst,
                IgstPercentage = igst,
                TaxAmount = itemTax
            };
        }).ToList();

        // Group all tax lines into summary
        var taxBreakdown = allTaxLines
            .GroupBy(t => new { t.TaxType, t.TaxPercentage })
            .Select(g => new TaxSummaryDto
            {
                TaxType = g.Key.TaxType,
                TaxPercentage = g.Key.TaxPercentage,
                TotalTaxAmount = Math.Round(g.Sum(t => t.TotalTaxAmount), 2)
            }).ToList();

        var totalTax = taxBreakdown.Sum(t => t.TotalTaxAmount);
        var grandTotal = subtotalAfterDiscount + totalTax;

        // Get payments
        var totalPaid = await _context.BanquetPayments
            .Where(p => p.BanquetBookingId == bookingId && p.DeletedAt == null)
            .SumAsync(p => p.Amount);

        return new BanquetBillPreviewDto
        {
            BanquetBookingId = bookingId,
            BookingNumber = booking.BookingNumber,
            HallName = booking.BanquetHall.HallName,
            EventTypeName = booking.EventType.EventTypeName,
            EventDate = booking.EventDate,
            ContactPersonName = booking.ContactPersonName,
            CompanyName = booking.Company?.CompanyName,
            ExpectedGuests = booking.ExpectedGuests,
            ActualGuests = booking.ActualGuests,
            HallRent = booking.HallRent,
            MenuCharges = menuCharges,
            MenuChargesSubtotal = menuChargesSubtotal,
            ServiceCharges = serviceCharges,
            ServiceChargesSubtotal = serviceChargesSubtotal,
            AdditionalCharges = additionalCharges,
            AdditionalChargesSubtotal = additionalChargesSubtotal,
            Subtotal = subtotal,
            DiscountPercentage = booking.DiscountPercentage,
            DiscountAmount = discountAmount,
            SubtotalAfterDiscount = subtotalAfterDiscount,
            TaxBreakdown = taxBreakdown,
            TotalTax = totalTax,
            GrandTotal = grandTotal,
            TotalPaid = totalPaid,
            BalanceDue = grandTotal - totalPaid,
            GeneratedAt = DateTime.UtcNow
        };
    }

    private static void CalculateVoucherTax(
        decimal taxableAmount,
        TaxType taxType,
        VoucherTaxConfiguration? voucherTaxConfig,
        decimal fallbackCgst,
        decimal fallbackSgst,
        decimal fallbackIgst,
        List<TaxSummaryDto> taxLines,
        out decimal totalItemTax)
    {
        totalItemTax = 0;
        if (taxableAmount <= 0) return;

        if (taxType == TaxType.CgstSgst)
        {
            var cgstPct = voucherTaxConfig?.CgstPercentage ?? fallbackCgst;
            var sgstPct = voucherTaxConfig?.SgstPercentage ?? fallbackSgst;

            var cgstAmt = Math.Round(taxableAmount * cgstPct / 100, 2);
            var sgstAmt = Math.Round(taxableAmount * sgstPct / 100, 2);
            totalItemTax = cgstAmt + sgstAmt;

            if (cgstAmt > 0)
                taxLines.Add(new TaxSummaryDto { TaxType = "CGST", TaxPercentage = cgstPct, TotalTaxAmount = cgstAmt });
            if (sgstAmt > 0)
                taxLines.Add(new TaxSummaryDto { TaxType = "SGST", TaxPercentage = sgstPct, TotalTaxAmount = sgstAmt });
        }
        else
        {
            var igstPct = voucherTaxConfig?.IgstPercentage ?? fallbackIgst;
            var igstAmt = Math.Round(taxableAmount * igstPct / 100, 2);
            totalItemTax = igstAmt;

            if (igstAmt > 0)
                taxLines.Add(new TaxSummaryDto { TaxType = "IGST", TaxPercentage = igstPct, TotalTaxAmount = igstAmt });
        }
    }

    public async Task<BanquetInvoiceDto> FinalizeInvoiceAsync(int bookingId, FinalizeBanquetInvoiceDto dto)
    {
        var booking = await _context.BanquetBookings
            .Include(b => b.BanquetHall)
            .Include(b => b.EventType)
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeletedAt == null);

        if (booking == null)
            throw new NotFoundException(nameof(BanquetBooking), bookingId);

        if (booking.Status == BanquetBookingStatus.Cancelled)
            throw new BusinessRuleException("Cannot finalize invoice for a cancelled booking");

        // Check if invoice already exists
        var existingInvoice = await _context.BanquetInvoices
            .FirstOrDefaultAsync(i => i.BanquetBookingId == bookingId && i.DeletedAt == null);

        if (existingInvoice != null)
            throw new BusinessRuleException("Invoice already exists for this booking");

        // Update actual guests if provided
        if (dto.ActualGuests.HasValue)
            booking.ActualGuests = dto.ActualGuests;

        // Get bill preview
        var preview = await GetBillPreviewAsync(bookingId);

        // Get payment history
        var payments = await _context.BanquetPayments
            .Where(p => p.BanquetBookingId == bookingId && p.DeletedAt == null)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();

        var paymentHistory = payments.Select(p => new BanquetPaymentHistoryDto
        {
            ReceiptNumber = p.ReceiptNumber,
            PaymentDate = p.PaymentDate,
            PaymentType = p.PaymentType.ToString(),
            PaymentMode = p.PaymentMode.ToString(),
            Amount = p.Amount,
            ReferenceNumber = p.ReferenceNumber
        }).ToList();

        // Generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        // Determine payment status
        var paymentStatus = preview.BalanceDue <= 0 ? "Paid" : preview.TotalPaid > 0 ? "Partial" : "Unpaid";

        var invoice = new BanquetInvoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow,
            BanquetBookingId = bookingId,
            HallName = booking.BanquetHall.HallName,
            EventTypeName = booking.EventType.EventTypeName,
            EventDate = booking.EventDate,
            ContactPersonName = booking.ContactPersonName,
            CompanyName = booking.Company?.CompanyName,
            ExpectedGuests = booking.ExpectedGuests,
            ActualGuests = booking.ActualGuests,
            MenuChargesJson = JsonSerializer.Serialize(preview.MenuCharges),
            ServiceChargesJson = preview.ServiceCharges.Any()
                ? JsonSerializer.Serialize(preview.ServiceCharges) : null,
            AdditionalChargesJson = preview.AdditionalCharges.Any()
                ? JsonSerializer.Serialize(preview.AdditionalCharges) : null,
            TaxBreakdownJson = JsonSerializer.Serialize(preview.TaxBreakdown),
            PaymentHistoryJson = paymentHistory.Any()
                ? JsonSerializer.Serialize(paymentHistory) : null,
            HallRent = preview.HallRent,
            MenuChargesSubtotal = preview.MenuChargesSubtotal,
            ServiceChargesSubtotal = preview.ServiceChargesSubtotal,
            AdditionalChargesSubtotal = preview.AdditionalChargesSubtotal,
            DiscountAmount = preview.DiscountAmount,
            SubtotalBeforeTax = preview.SubtotalAfterDiscount,
            TotalTax = preview.TotalTax,
            GrandTotal = preview.GrandTotal,
            TotalPaid = preview.TotalPaid,
            BalanceDue = preview.BalanceDue,
            PaymentStatus = paymentStatus
        };

        _context.BanquetInvoices.Add(invoice);

        // Update booking status to Completed
        if (booking.Status != BanquetBookingStatus.Completed)
        {
            booking.Status = BanquetBookingStatus.Completed;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Banquet invoice {InvoiceNumber} created for booking {BookingId}",
            invoiceNumber, bookingId);

        return await GetInvoiceByIdAsync(invoice.Id)
            ?? throw new Exception("Failed to retrieve created invoice");
    }

    public async Task<BanquetInvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _context.BanquetInvoices
            .Include(i => i.BanquetBooking)
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.DeletedAt == null);

        if (invoice == null) return null;

        return MapInvoiceToDto(invoice);
    }

    public async Task<BanquetInvoiceDto?> GetInvoiceByBookingIdAsync(int bookingId)
    {
        var invoice = await _context.BanquetInvoices
            .Include(i => i.BanquetBooking)
            .FirstOrDefaultAsync(i => i.BanquetBookingId == bookingId && i.DeletedAt == null);

        if (invoice == null) return null;

        return MapInvoiceToDto(invoice);
    }

    public async Task<List<BanquetInvoiceListDto>> GetAllInvoicesAsync()
    {
        var invoices = await _context.BanquetInvoices
            .Include(i => i.BanquetBooking)
            .Where(i => i.DeletedAt == null)
            .OrderByDescending(i => i.InvoiceDate)
            .Select(i => new BanquetInvoiceListDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                BanquetBookingId = i.BanquetBookingId,
                BookingNumber = i.BanquetBooking.BookingNumber,
                HallName = i.HallName,
                EventTypeName = i.EventTypeName,
                EventDate = i.EventDate,
                ContactPersonName = i.ContactPersonName,
                CompanyName = i.CompanyName,
                TotalTax = i.TotalTax,
                GrandTotal = i.GrandTotal,
                TotalPaid = i.TotalPaid,
                BalanceDue = i.BalanceDue,
                PaymentStatus = i.PaymentStatus
            })
            .ToListAsync();

        return invoices;
    }

    private BanquetInvoiceDto MapInvoiceToDto(BanquetInvoice invoice)
    {
        return new BanquetInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            BanquetBookingId = invoice.BanquetBookingId,
            BookingNumber = invoice.BanquetBooking.BookingNumber,
            HallName = invoice.HallName,
            EventTypeName = invoice.EventTypeName,
            EventDate = invoice.EventDate,
            ContactPersonName = invoice.ContactPersonName,
            CompanyName = invoice.CompanyName,
            ExpectedGuests = invoice.ExpectedGuests,
            ActualGuests = invoice.ActualGuests,
            HallRent = invoice.HallRent,
            MenuCharges = JsonSerializer.Deserialize<List<BanquetMenuChargeDto>>(invoice.MenuChargesJson) ?? new(),
            MenuChargesSubtotal = invoice.MenuChargesSubtotal,
            ServiceCharges = !string.IsNullOrEmpty(invoice.ServiceChargesJson)
                ? JsonSerializer.Deserialize<List<BanquetServiceChargeDto>>(invoice.ServiceChargesJson) ?? new()
                : new(),
            ServiceChargesSubtotal = invoice.ServiceChargesSubtotal,
            AdditionalCharges = !string.IsNullOrEmpty(invoice.AdditionalChargesJson)
                ? JsonSerializer.Deserialize<List<BanquetAdditionalChargeDto>>(invoice.AdditionalChargesJson) ?? new()
                : new(),
            AdditionalChargesSubtotal = invoice.AdditionalChargesSubtotal,
            DiscountAmount = invoice.DiscountAmount,
            SubtotalBeforeTax = invoice.SubtotalBeforeTax,
            TaxBreakdown = JsonSerializer.Deserialize<List<TaxSummaryDto>>(invoice.TaxBreakdownJson) ?? new(),
            TotalTax = invoice.TotalTax,
            GrandTotal = invoice.GrandTotal,
            TotalPaid = invoice.TotalPaid,
            BalanceDue = invoice.BalanceDue,
            PaymentStatus = invoice.PaymentStatus,
            PaymentHistory = !string.IsNullOrEmpty(invoice.PaymentHistoryJson)
                ? JsonSerializer.Deserialize<List<BanquetPaymentHistoryDto>>(invoice.PaymentHistoryJson) ?? new()
                : new(),
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy
        };
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"BINV-{year}-";

        var lastInvoice = await _context.BanquetInvoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastInvoice != null)
        {
            var lastNumberStr = lastInvoice.InvoiceNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D5}";
    }
}
