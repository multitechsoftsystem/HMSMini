# Day Close Tax Calculation Fix

## Problem Identified

The Day Close Preview was showing incorrect amounts for rooms because it was using a **hardcoded 18% GST rate** instead of reading the actual tax rate from the guest's tax slab configuration.

### Before Fix:

**DayClosingService.cs (Line 188):**
```csharp
// Simplified tax estimate (18% = 9% CGST + 9% SGST or 18% IGST)
decimal taxAmount = Math.Round(taxableAmount * 0.18m, 2);
```

### Example Discrepancies:

#### Room 404:
- **Expected**: ₹2,500 base + 5% tax = ₹2,625
- **Showing**: ₹2,950 (using 18% tax)
- **Error**: ₹325 excess

#### Room 302:
- **Expected**: ₹6,000 base (₹3,000 room + ₹3,000 meal) + 5% tax = ₹6,300
- **Showing**: ₹7,080 (using 18% tax)
- **Error**: ₹780 excess

## Solution

Updated `DayClosingService.cs` to deserialize the `TaxSlabSnapshot` from each check-in and calculate tax using the **actual tax rate** configured for that guest, matching the logic used in `VoucherService.cs`.

### Changes Made:

1. **Parse TaxSlabSnapshot**: Deserialize the JSON stored in `CheckIn.TaxSlabSnapshotJson`
2. **Find Applicable Slab**: Match the taxable amount against the configured tax slabs
3. **Calculate Actual Tax**: Use the slab's CGST/SGST/IGST percentages (e.g., 2.5% + 2.5% = 5% total)
4. **Error Handling**: Added try-catch with logging for snapshot deserialization failures

### After Fix:

The Day Close Preview now:
- ✅ Uses the **correct tax rate** for each guest (5%, 12%, 18%, etc.)
- ✅ Matches the **Bill Preview** calculations exactly
- ✅ Matches the **actual voucher posting** logic
- ✅ Handles different tax types (CGST/SGST vs IGST)

## Expected Behavior After Fix

### Room 404 (Base ₹2,500, 5% GST):
- Room Tariff: ₹2,500
- CGST 2.5%: ₹62.50
- SGST 2.5%: ₹62.50
- **Total in Day Close**: ₹2,625 (3 vouchers)

### Room 302 (Base ₹3,000 + Meal ₹3,000, 5% GST):
- Room Tariff: ₹3,000
- Meal Plan: ₹3,000
- Taxable: ₹6,000
- CGST 2.5%: ₹150
- SGST 2.5%: ₹150
- **Total in Day Close**: ₹6,300 (4 vouchers)

## Testing

### To verify the fix:
1. Restart the API
2. Navigate to Day Closing page
3. Click "Refresh" to regenerate the preview
4. Verify amounts match the Bill Preview for each room

### APIs Involved:
- `GET /api/DayClose/preview` - Uses fixed DayClosingService
- `GET /api/CheckIns/{id}/bill-preview` - Reference for correct amounts

## Files Modified

- `src/HMSMini.API/Services/Implementations/DayClosingService.cs`
  - Lines 178-238: Replaced hardcoded tax calculation with TaxSlabSnapshot-based logic

## Deployment

Run the following commands to deploy:
```bash
cd src/HMSMini.API
dotnet build
dotnet run
```

Or restart the API service if already running.

---

**Fixed by:** Claude Code
**Date:** 2026-01-29
**Build Status:** ✅ Success (0 errors, 0 warnings)
