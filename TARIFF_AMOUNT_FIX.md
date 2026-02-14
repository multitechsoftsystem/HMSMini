# Tariff Amount Issue - Root Cause and Fix

## Issue Summary

**Problem**: Day closing shows ₹0.00 total revenue even with 6 active check-ins. No room tariff or meal plan charges are being posted.

**Screenshot Evidence**: Day Close Preview showing:
- 6 active check-ins
- 4 vouchers (only tax vouchers with ₹0.00 amounts)
- Total Revenue: ₹0.00

---

## Root Cause Analysis

### Problem in CheckInService.cs (Line 221)

```csharp
// OLD CODE (INCORRECT):
if (dto.CompanyId.HasValue || dto.MealPlanId.HasValue)
{
    // Calculate tariff ONLY if company OR meal plan provided
    var tariffCalc = await _tariffService.CalculateTariffAsync(...);
    tariffApplied = tariffCalc.ApplicableRate;
}
```

**This caused**:
- ❌ Walk-in guests **WITHOUT** company **AND WITHOUT** meal plan → TariffApplied = **NULL**
- ❌ Day closing → No room tariff voucher generated (because TariffApplied is NULL)
- ❌ No base amount → Tax calculated on ₹0.00 → Tax vouchers with ₹0.00

### VoucherService Logic (Correct but dependent on TariffApplied)

```csharp
// VoucherService.cs Line 171
if (checkIn.TariffApplied.HasValue && checkIn.TariffApplied.Value > 0)
{
    // Create room tariff voucher
}
```

**If TariffApplied is NULL → No voucher created** ✅ This logic is correct!

---

## Solution Implemented

### 1. Fixed CheckInService.cs

**Changed**: Now calculates tariff for **ALL guests**, not just corporate or with meal plan

```csharp
// NEW CODE (CORRECT):
// Calculate tariff for all guests (walk-in, corporate, with/without meal plan)
var roomForTariff = await _context.Rooms
    .Include(r => r.RoomType)
    .FirstOrDefaultAsync(r => r.RoomId == roomId);

if (roomForTariff != null)
{
    var tariffCalc = await _tariffService.CalculateTariffAsync(
        roomForTariff.RoomTypeId,
        dto.Guests.Count,
        dto.CheckInDate,
        dto.CheckOutDate,
        dto.CompanyId,  // Can be NULL for walk-in
        dto.MealPlanId); // Can be NULL for no meal plan

    tariffApplied = tariffCalc.ApplicableRate;
    // ... rest of calculation
}
```

### 2. Impact

✅ **New check-ins** created after this fix will have tariff calculated correctly

❌ **Existing check-ins** in database still have TariffApplied = NULL

---

## Fixing Existing Check-ins

You have **TWO OPTIONS**:

### Option 1: Run SQL Script (Recommended)

**File**: `fix_existing_checkins_tariff.sql`

This script will:
1. Find all check-ins with NULL TariffApplied
2. Look up the base tariff from BaseTariffs table
3. Update TariffApplied and FinalAmount

**How to run**:
```sql
-- Connect to your database using SQL Server Management Studio or sqlcmd
-- Run the script:
sqlcmd -S your_server -d your_database -i fix_existing_checkins_tariff.sql
```

**Or in SSMS**: Open the file and execute it

### Option 2: Update Check-ins via API

For each check-in, use the **Update Check-in API** endpoint to modify and re-calculate tariff:
- PUT `/api/checkins/{id}`
- The update will recalculate the tariff

---

## Verification Steps

After applying the fix:

### 1. Check Database

```sql
-- View all active check-ins with their tariffs
SELECT
    c.Id,
    r.RoomNumber,
    rt.RoomType,
    c.Pax,
    c.TariffApplied,
    c.MealPlanRate,
    c.FinalAmount,
    c.Status
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN MRoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
WHERE c.Status = 0  -- Active
ORDER BY r.RoomNumber
```

Expected result: All check-ins should have TariffApplied > 0

### 2. Test Day Closing Preview

1. Login to system
2. Go to Day Closing page
3. Click "Preview"
4. **Expected result**:
   - Each check-in shows > 0 vouchers
   - Room Tariff amount > ₹0.00
   - Meal Plan amount (if applicable) > ₹0.00
   - Tax amounts calculated correctly
   - **Total Revenue > ₹0.00**

### 3. Test New Check-in

1. Create a new walk-in check-in WITHOUT company and WITHOUT meal plan
2. Check the database:
   ```sql
   SELECT TOP 1 * FROM CheckIn ORDER BY Id DESC
   ```
3. **Expected**: TariffApplied should have a value (not NULL)

---

## Example Calculation

### Scenario:
- **Room**: 404 (Deluxe Room)
- **Pax**: 2 guests
- **Check-in**: 27-Jan-2026
- **Checkout**: 30-Jan-2026
- **Guest Type**: Walk-in (no company, no meal plan)

### Before Fix:
```
TariffApplied = NULL
MealPlanRate = NULL
FinalAmount = NULL

Day Close for 27-Jan:
  - Room Tariff: SKIPPED (NULL tariff)
  - Tax: ₹0.00 (no base amount)
  Total: ₹0.00 ❌
```

### After Fix:
```
TariffApplied = ₹2500 (from BaseTariffs for Deluxe Room, 2 pax)
MealPlanRate = NULL
FinalAmount = ₹2500

Day Close for 27-Jan:
  - Room Tariff: ₹2500
  - CGST (9%): ₹225
  - SGST (9%): ₹225
  Total: ₹2950 ✅
```

---

## Important Notes

1. **BaseTariffs Required**: The system needs base tariffs configured in the `BaseTariffs` table for each room type

2. **Tax Configuration**: Ensure tax slabs are configured in `TaxSlabs` table

3. **Complimentary Guests**: Guests with GuestType = "Complimentary" will still have ₹0.00 (this is correct behavior)

4. **Future Check-ins**: All new check-ins will automatically calculate tariffs correctly

---

## Files Modified

1. `src/HMSMini.API/Services/Implementations/CheckInService.cs`
   - Line 221: Removed condition that required CompanyId or MealPlanId
   - Now calculates tariff for ALL guests

2. `src/HMSMini.API/Services/Implementations/DayClosingService.cs`
   - Already updated to respect ActualCheckOutDate

---

## Summary

**Root Cause**: Tariffs only calculated for corporate guests or guests with meal plan

**Fix**: Calculate tariffs for ALL guests (walk-in, corporate, with/without meal plan)

**Action Required**: Run SQL script to fix existing check-ins OR update them via API

**Expected Result**: Day closing will show correct amounts with room tariff + meal plan + taxes

---

**Last Updated**: 29-Jan-2026 22:05
**Version**: 1.0
**Status**: ✅ Fixed - API redeployed with correct logic
