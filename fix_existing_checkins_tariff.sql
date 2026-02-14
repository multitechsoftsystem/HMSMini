-- =====================================================
-- Fix Existing Check-ins: Add Missing Tariff Amounts
-- =====================================================
-- This script updates existing check-ins that have NULL TariffApplied
-- by calculating and setting the appropriate base tariff

-- IMPORTANT: This assumes you have base tariffs set up in BaseTariffs table

PRINT 'Starting tariff update for existing check-ins...'
PRINT ''

-- Check current state
PRINT 'Current check-ins without tariff:'
SELECT
    c.Id,
    c.RoomId,
    r.RoomNumber,
    rt.RoomType,
    c.Pax,
    c.CheckInDate,
    c.CheckOutDate,
    c.TariffApplied,
    c.MealPlanRate,
    c.Status
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN MRoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
WHERE c.TariffApplied IS NULL AND c.Status = 0
ORDER BY c.CheckInDate DESC

PRINT ''
PRINT 'Updating check-ins with base tariff...'

-- Update check-ins with base tariff from BaseTariffs table
UPDATE c
SET
    c.TariffApplied = bt.BaseRate,
    c.FinalAmount = CASE
        WHEN c.MealPlanRate IS NOT NULL THEN bt.BaseRate + c.MealPlanRate
        ELSE bt.BaseRate
    END,
    c.UpdatedAt = GETUTCDATE(),
    c.UpdatedBy = 'System-TariffFix'
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN MRoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
-- Join with BaseTariffs to get the base rate for the room type
LEFT JOIN BaseTariffs bt ON bt.RoomTypeId = rt.RoomTypeId
    AND bt.IsActive = 1
    AND c.Pax BETWEEN bt.MinOccupancy AND bt.MaxOccupancy
WHERE
    c.TariffApplied IS NULL
    AND c.Status = 0  -- Only active check-ins
    AND bt.BaseRate IS NOT NULL  -- Only if base tariff exists

PRINT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' check-in(s) with base tariff'
PRINT ''

-- Check for check-ins that still don't have tariff (no matching base tariff)
PRINT 'Check-ins still without tariff (no matching base tariff found):'
SELECT
    c.Id,
    r.RoomNumber,
    rt.RoomType,
    c.Pax,
    c.Status
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN MRoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
WHERE c.TariffApplied IS NULL AND c.Status = 0

-- If there are still check-ins without tariff, you need to either:
-- 1. Add base tariffs for those room types in BaseTariffs table
-- 2. Manually set the tariff for those specific check-ins

PRINT ''
PRINT 'Final state of active check-ins:'
SELECT
    c.Id,
    r.RoomNumber,
    rt.RoomType,
    c.Pax,
    c.TariffApplied as RoomTariff,
    c.MealPlanRate,
    c.FinalAmount,
    c.Status
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN MRoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
WHERE c.Status = 0
ORDER BY r.RoomNumber

PRINT ''
PRINT 'Tariff update completed!'

-- =====================================================
-- Alternative: Manual update for specific check-ins
-- =====================================================
-- If you know the tariff for specific rooms, you can manually set them:
-- UPDATE CheckIn SET TariffApplied = 1000, FinalAmount = 1000, UpdatedAt = GETUTCDATE(), UpdatedBy = 'Admin'
-- WHERE RoomId = (SELECT RoomId FROM RoomNo WHERE RoomNumber = '101')
-- AND Status = 0 AND TariffApplied IS NULL
