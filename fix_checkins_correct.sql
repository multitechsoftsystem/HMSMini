-- =====================================================
-- Fix Existing Check-ins: Add Missing Tariff Amounts
-- =====================================================

PRINT 'Starting tariff update for existing check-ins...'
PRINT ''

-- Update check-ins with base tariff
UPDATE c
SET
    c.TariffApplied = bt.RatePerNight,
    c.FinalAmount = CASE
        WHEN c.MealPlanRate IS NOT NULL THEN bt.RatePerNight + c.MealPlanRate
        ELSE bt.RatePerNight
    END,
    c.UpdatedAt = GETUTCDATE(),
    c.UpdatedBy = 'System-TariffFix'
FROM CheckIn c
INNER JOIN RoomNo r ON c.RoomId = r.RoomId
INNER JOIN BaseTariff bt ON bt.RoomTypeId = r.RoomTypeId
    AND bt.OccupancyCount = c.Pax
    AND bt.IsActive = 1
WHERE
    c.TariffApplied IS NULL
    AND c.Status = 0  -- Only active check-ins

PRINT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' check-in(s)'
PRINT ''

-- Show final results
PRINT 'Active check-ins with tariffs:'
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
WHERE c.Status = 0
ORDER BY r.RoomNumber

PRINT ''
PRINT 'Tariff update completed!'
