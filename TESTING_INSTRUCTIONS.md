# Testing Instructions for Day Close Tax Fix

## API Status
✅ **API is running on:** http://localhost:5096

## How to Test the Fix

### Option 1: Test via Web UI (Recommended)

1. **Open the Blazor Web Application:**
   - Navigate to the web UI (typically running on a different port)
   - Login with your credentials

2. **Navigate to Day Closing Page:**
   - Click on "Day Closing" in the navigation menu
   - OR directly access: http://localhost:{web-port}/dayclosing

3. **Refresh the Day Close Preview:**
   - Click the **"Refresh"** button at the top right
   - This will regenerate the preview using the fixed calculation

4. **Verify the Amounts:**

   **Expected Results:**

   **Room 404 (Make my trip 1):**
   - Base Tariff: ₹2,500
   - CGST 2.5%: ₹62.50
   - SGST 2.5%: ₹62.50
   - **Total: ₹2,625.00** (was showing ₹2,950.00)
   - Vouchers: 3 (1 Room + 1 CGST + 1 SGST)

   **Room 302 (Make my trip 2):**
   - Base Tariff: ₹3,000
   - Meal Plan: ₹3,000
   - Subtotal: ₹6,000
   - CGST 2.5%: ₹150.00
   - SGST 2.5%: ₹150.00
   - **Total: ₹6,300.00** (was showing ₹7,080.00)
   - Vouchers: 4 (1 Room + 1 Meal + 1 CGST + 1 SGST)

### Option 2: Test via API Endpoint

1. **Get Day Close Preview via API:**
   ```bash
   curl http://localhost:5096/api/DayClose/preview \
     -H "Authorization: Bearer YOUR_TOKEN"
   ```

2. **Check the Response:**
   - Look at the `checkInSummaries` array
   - Find Room 404 and Room 302
   - Verify the `amountToPost` matches expected values
   - Check `voucherSummary` for tax amounts

### Option 3: Compare with Bill Preview

1. **Get Bill Preview for Room 404:**
   ```bash
   curl http://localhost:5096/api/CheckIns/{room404_checkInId}/bill-preview \
     -H "Authorization: Bearer YOUR_TOKEN"
   ```

2. **Get Bill Preview for Room 302:**
   ```bash
   curl http://localhost:5096/api/CheckIns/{room302_checkInId}/bill-preview \
     -H "Authorization: Bearer YOUR_TOKEN"
   ```

3. **Compare:**
   - The amounts in Day Close Preview should now **match exactly** with the Bill Preview
   - Tax percentages should be the same (5% total = 2.5% CGST + 2.5% SGST)

## What Changed?

### Before Fix:
- Day Close Preview used a **hardcoded 18% GST rate**
- This caused amounts to be higher than actual billing
- Did not match Bill Preview calculations

### After Fix:
- Day Close Preview now reads the **actual tax slab** from `TaxSlabSnapshot`
- Calculates tax using the correct rate configured for each guest
- Matches Bill Preview and Voucher Posting logic exactly

## Troubleshooting

### If amounts still look wrong:

1. **Clear browser cache** and refresh
2. **Check the Tax Slab Configuration:**
   - Navigate to Tax Slabs management
   - Verify that the correct tax rates are configured (e.g., 5% = 2.5% CGST + 2.5% SGST)

3. **Check the Check-In's Tax Type:**
   - Get check-in details: `GET /api/CheckIns/{id}`
   - Look at `taxType` field (should be "Cgst" or "Igst")
   - Look at `taxSlabSnapshotJson` to see what tax rate was captured at check-in

4. **Verify the stored tariff:**
   - Check `tariffApplied` field in check-in
   - Should be ₹2,500 for Room 404
   - Should be ₹3,000 for Room 302

### If you need to update the tariff:

Use the Update Check-In API:
```http
PUT /api/CheckIns/{id}
Content-Type: application/json

{
  "CompanyId": null,
  "BusinessSourceId": null,
  "MealPlanId": null,
  "GuestTypeId": null,
  "Remarks": null,
  "CheckOutDate": null
}
```

**Note:** Currently there's no direct API to update the tariff. If the wrong tariff was stored, you may need to:
1. Check out the guest
2. Create a new check-in with the correct tariff

## Summary

✅ **Fix Applied:** Tax calculation now uses actual tax slabs
✅ **API Running:** http://localhost:5096
✅ **Expected Behavior:** Day Close amounts match Bill Preview

**Next Step:** Test in the web UI by clicking "Refresh" on the Day Closing page and verify the amounts.

---
Generated: 2026-01-29 19:05
