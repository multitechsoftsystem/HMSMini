# Tariff Posting Logic - Day Closing

## Overview
The day closing process posts tariffs for each night a guest occupies a room. Tariffs are posted **for the working date being closed**, and the system respects early checkouts.

## Key Principle
**Tariff for date X is posted when you run day closing for date X**

## Example Scenario

### Guest Check-in Details:
- **Room**: 404
- **Check-in Date**: 27-Jan-2026
- **Expected Checkout**: 30-Jan-2026
- **System Date**: 28-Jan-2026

### Day-by-Day Tariff Posting:

#### 27-Jan-2026 (Check-in Day)
- Guest checks in at 2:00 PM
- Occupies room on night of 27-Jan
- **No tariff posted yet** (guest just checked in)

#### 28-Jan-2026 (First Day Close)
- Run **Day Close for 27-Jan-2026**
- ✅ **Posts tariff for 27-Jan-2026** (first night)
- Working date advances to 28-Jan-2026

#### 29-Jan-2026 (Second Day Close)
- Run **Day Close for 28-Jan-2026**
- ✅ **Posts tariff for 28-Jan-2026** (second night)
- Working date advances to 29-Jan-2026

#### 30-Jan-2026 (Third Day Close)
- Run **Day Close for 29-Jan-2026**
- ✅ **Posts tariff for 29-Jan-2026** (third night)
- Working date advances to 30-Jan-2026
- Guest checks out at 11:00 AM - **no charge for 30-Jan**

### Total Charges:
- 3 nights × Room tariff = Total tariff
- Plus meal plan charges (if applicable)
- Plus applicable taxes

---

## Early Checkout Scenario

### Modified Example:
- **Room**: 404
- **Check-in Date**: 27-Jan-2026
- **Expected Checkout**: 30-Jan-2026
- **Actual Checkout**: 29-Jan-2026 (11:00 AM)

### Day-by-Day Posting:

#### 28-Jan-2026
- Run **Day Close for 27-Jan-2026**
- ✅ **Posts tariff for 27-Jan** (guest was in house)
- Working date → 28-Jan-2026

#### 29-Jan-2026
- Guest checks out at 11:00 AM (ActualCheckOutDate = 29-Jan-2026)
- Run **Day Close for 28-Jan-2026**
- ✅ **Posts tariff for 28-Jan** (guest was still in house on 28-Jan)
- Working date → 29-Jan-2026

#### 30-Jan-2026
- Run **Day Close for 29-Jan-2026**
- ❌ **No tariff for 29-Jan** (guest checked out in the morning before occupying room on 29-Jan night)
- Working date → 30-Jan-2026

### Total Charges:
- **2 nights only** (27-Jan and 28-Jan)
- Guest not charged for 29-Jan or 30-Jan

---

## Overstay Scenario

### Modified Example:
- **Room**: 404
- **Check-in Date**: 27-Jan-2026
- **Expected Checkout**: 29-Jan-2026
- **Actual Checkout**: Guest doesn't check out (overstays)

### Day-by-Day Posting:

#### 28-Jan-2026
- Run **Day Close for 27-Jan-2026**
- ✅ **Posts tariff for 27-Jan**
- Working date → 28-Jan-2026

#### 29-Jan-2026
- Run **Day Close for 28-Jan-2026**
- ✅ **Posts tariff for 28-Jan**
- Working date → 29-Jan-2026
- Guest hasn't checked out yet (past expected checkout 29-Jan)

#### 30-Jan-2026
- Run **Day Close for 29-Jan-2026**
- System detects: Expected checkout (29-Jan) < Working date (29-Jan)
- **Auto-extends checkout date to 29-Jan**
- ✅ **Posts tariff for 29-Jan** (overstay charge)
- Working date → 30-Jan-2026

### Result:
- Guest charged for all nights they actually stayed
- System automatically extends checkout for overstay guests
- Management can see extension in day closing audit log

---

## Technical Implementation

### Query Logic (from DayClosingService.cs):

```csharp
// Get check-ins that were active on working date
var activeCheckIns = await _context.CheckIns
    .Where(c => c.Status == 0) // Active status
    .Where(c => c.CheckInDate.Date <= workingDate  // Checked in on or before working date
        && (c.ActualCheckOutDate == null           // Haven't checked out yet
            || c.ActualCheckOutDate.Value.Date > workingDate)) // OR checked out after working date
    .ToListAsync();
```

### Key Fields:
- **CheckInDate**: When guest checked in
- **CheckOutDate**: Expected checkout date
- **ActualCheckOutDate**: Actual checkout date (null if still checked in)
- **Status**: Active (0) or CheckedOut (1)

### Posting Rules:
1. Post tariff if: `CheckInDate <= WorkingDate`
2. AND: Guest was in house on working date:
   - `ActualCheckOutDate == null` (not checked out yet)
   - OR `ActualCheckOutDate > WorkingDate` (checked out after working date)
3. Auto-extend if: `CheckOutDate < WorkingDate` AND `ActualCheckOutDate == null`

---

## Benefits of This Approach

✅ **Accurate Charging**: Only charge for nights actually occupied
✅ **Early Checkout Support**: Automatic refund for unused nights
✅ **Overstay Handling**: Automatic extension and charging for overstays
✅ **Audit Trail**: Complete record of all tariff postings
✅ **Tax Compliance**: Proper date-wise revenue posting for accounting

---

## Important Notes

1. **Check-out Day No Charge**: Hotel standard - no charge for checkout day (e.g., checkout on 30-Jan = no charge for 30-Jan)

2. **Day Closing Sequence**: Always close days in sequence (cannot skip days)

3. **Validation**: System prevents closing future dates (working date must be < system date)

4. **Complimentary Guests**: No tariff posted for guest type = "Complimentary"

5. **Meal Plans**: Posted separately but follow same date logic as room tariff

6. **Taxes**: Calculated and posted based on tariff + meal plan for each night

---

## Summary Table

| Scenario | Check-in | Expected Checkout | Actual Checkout | Nights Charged |
|----------|----------|------------------|-----------------|----------------|
| Normal Stay | 27-Jan | 30-Jan | 30-Jan | 27, 28, 29 (3 nights) |
| Early Checkout | 27-Jan | 30-Jan | 29-Jan | 27, 28 (2 nights) |
| Overstay | 27-Jan | 29-Jan | 31-Jan | 27, 28, 29, 30 (4 nights) |
| Same Day Checkout | 27-Jan | 27-Jan | 27-Jan | 0 nights (no charge) |

---

**Last Updated**: 28-Jan-2026
**Version**: 1.0
