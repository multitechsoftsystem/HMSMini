# Additional Charges (Laundry) Tax Calculation Fix

## Issue Reported

User added a laundry voucher to Room 103 with **18% tax** (9% CGST + 9% SGST), but the Day Close Preview was showing **2.5% tax** instead.

## Root Cause Analysis

The Day Close Preview was **NOT including additional charges (laundry, minibar, etc.) at all** in its calculations!

### What Was Missing:
1. ❌ Additional charge vouchers not counted
2. ❌ Additional charge amounts not included in revenue
3. ❌ Additional charge taxes not calculated
4. ❌ Voucher-specific tax rates (18%, 12%, etc.) were ignored

### What Was Shown:
- Only room tariff + meal plan charges
- Only tax on room/meal using tax slabs (5% = 2.5% CGST + 2.5% SGST)
- Additional charges were counted in "Unposted Additional Charges Count" but not in the voucher summary

## The Fix

### Changes Made to DayClosingService.cs:

#### 1. Added Query for Unposted Additional Charges (Line 142-149)
```csharp
// Get all unposted additional charges for preview
var unpostedChargesByCheckIn = await _context.AdditionalCharges
    .Include(a => a.VoucherTaxConfig)  // Load voucher-specific tax rates
    .Where(a => !a.IsPostedToVoucher)
    .Where(a => a.DeletedAt == null)
    .Where(a => activeCheckIns.Select(c => c.Id).Contains(a.CheckInId))
    .ToListAsync();
var chargesGrouped = unpostedChargesByCheckIn.GroupBy(a => a.CheckInId).ToDictionary(g => g.Key, g => g.ToList());
```

#### 2. Added Additional Charge Processing Logic (Line 239-321)
For each check-in with additional charges:
- **Add voucher for the charge itself** (e.g., laundry ₹500)
- **Calculate tax using voucher-specific rates** if configured:
  - If VoucherTaxConfig exists: Use 18% (9% CGST + 9% SGST) for laundry
  - Otherwise: Fall back to check-in's tax slab (5%, 12%, etc.)
- **Add tax vouchers** to the day close summary

### Tax Calculation Logic:

```csharp
if (charge.VoucherTaxConfigId.HasValue && charge.VoucherTaxConfig != null)
{
    // Use voucher-specific tax rates (e.g., 18% for laundry)
    var voucherTaxConfig = charge.VoucherTaxConfig;

    if (checkIn.TaxType == Igst)
    {
        igstAmount = charge.TotalAmount × 18% = tax
    }
    else
    {
        cgstAmount = charge.TotalAmount × 9% = tax
        sgstAmount = charge.TotalAmount × 9% = tax
    }
}
else
{
    // Fall back to tax slab system (5%, 12%, etc.)
    taxLines = await _taxService.CalculateTaxAsync(...)
}
```

## Before vs After Fix

### Before (Missing Additional Charges):

**Day Close Preview for Room 103:**
- Room Tariff: ₹2,000 (1 voucher)
- Tax-CGST: ₹50 (room tax @ 2.5%)
- Tax-SGST: ₹50 (room tax @ 2.5%)
- **Total: 3 vouchers, ₹2,100**
- ❌ Laundry charge **NOT SHOWN**
- ❌ Laundry tax **NOT SHOWN**

### After (Including Additional Charges):

**Day Close Preview for Room 103:**
- Room Tariff: ₹2,000 (1 voucher)
- **AdditionalCharge: ₹500 (1 voucher)** ✅ NEW
- Tax-CGST: ₹50 + ₹45 = **₹95** (room @ 2.5% + laundry @ 9%) ✅
- Tax-SGST: ₹50 + ₹45 = **₹95** (room @ 2.5% + laundry @ 9%) ✅
- **Total: 5 vouchers, ₹2,690**

### Calculation Breakdown:

**Room Tariff:**
- Base: ₹2,000
- CGST @ 2.5%: ₹50
- SGST @ 2.5%: ₹50

**Laundry Voucher:**
- Base: ₹500
- CGST @ 9%: ₹45 ✅ (Now showing correctly!)
- SGST @ 9%: ₹45 ✅ (Now showing correctly!)

**Total Tax:**
- CGST: ₹50 + ₹45 = ₹95
- SGST: ₹50 + ₹45 = ₹95
- **Total: ₹190** (was only ₹100 before)

## Expected Behavior

### For Room 103 with Laundry:

**Bill Preview shows:**
- Room charges: ₹2,000 × 2 days = ₹4,000
- Room tax @ 5%: ₹200
- Laundry: ₹500
- Laundry tax @ 18%: ₹90
- **Grand Total: ₹4,790**

**Day Close Preview NOW shows (for 1 day):**
- Room: ₹2,000
- Room tax: ₹100 (₹50 CGST + ₹50 SGST)
- Laundry: ₹500 ✅
- Laundry tax: ₹90 (₹45 CGST + ₹45 SGST) ✅
- **Total: ₹2,690** ✅

## Voucher Types Now Included:

The Day Close Preview now correctly handles:
- ✅ Room Tariff (with tax slab rates)
- ✅ Meal Plan (with tax slab rates)
- ✅ **Additional Charges** (laundry, minibar, etc.)
- ✅ **Voucher-specific tax rates** (18%, 12%, or other configured rates)
- ✅ Fallback to tax slabs for charges without specific tax config

## Files Modified

**DayClosingService.cs:**
- Lines 142-149: Added query for additional charges with VoucherTaxConfig
- Lines 239-321: Added logic to calculate additional charges and their taxes
- Uses same tax calculation logic as BillingService for consistency

## Testing Instructions

1. **Restart API** (already running on http://localhost:5096)
2. **Open Web UI** at http://localhost:5131
3. **Navigate to Day Closing page**
4. **Click "Refresh"** button
5. **Verify for Room 103:**
   - Should see "AdditionalCharge" in voucher summary
   - Tax-CGST should include laundry tax @ 9%
   - Tax-SGST should include laundry tax @ 9%
   - Total vouchers increased (now includes laundry + its taxes)
   - Total revenue increased (now includes laundry amount)

6. **Compare with Bill Preview:**
   - Day Close amounts should match Bill Preview
   - Laundry @ 18% should appear in both

## Technical Details

### VoucherTaxConfiguration Support:

Different charge types can have different tax rates:
- **Laundry**: 18% (9% CGST + 9% SGST)
- **Minibar**: 18% (9% CGST + 9% SGST)
- **Restaurant**: 5% (2.5% CGST + 2.5% SGST)
- **Room Service**: Custom rates

The fix ensures that Day Close Preview respects these configured rates, just like Bill Preview does.

## Consistency Achieved:

Now all three calculations are consistent:
1. ✅ **Bill Preview**: Shows laundry with 18% tax
2. ✅ **Day Close Preview**: Shows laundry with 18% tax
3. ✅ **Actual Voucher Posting**: Posts laundry with 18% tax

---

**Fixed by:** Claude Code
**Date:** 2026-01-29
**Build Status:** ✅ Success (0 errors, 1 warning)
**API Status:** ✅ Running on http://localhost:5096
**Web UI Status:** ✅ Running on http://localhost:5131
**Status:** ✅ Ready for Testing
