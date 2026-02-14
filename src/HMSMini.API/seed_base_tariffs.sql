-- Seed basic tariffs for all room types
-- Get room type IDs first and insert base tariffs

-- Single rooms
INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 1, 1500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Single' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 1
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 2, 2000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Single' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 2
);

-- Double rooms
INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 1, 2000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Double' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 1
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 2, 2500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Double' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 2
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 3, 3000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Double' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 3
);

-- Suite rooms
INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 1, 3000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Suite' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 1
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 2, 3500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Suite' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 2
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 3, 4000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Suite' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 3
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 4, 4500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Suite' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 4
);

-- Deluxe rooms
INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 1, 2500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Deluxe' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 1
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 2, 3000.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Deluxe' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 2
);

INSERT INTO BaseTariff (RoomTypeId, OccupancyCount, RatePerNight, EffectiveFrom, IsActive, CreatedAt, CreatedBy)
SELECT RoomTypeId, 3, 3500.00, '2025-01-01', 1, GETUTCDATE(), 'System'
FROM MRoomTypes WHERE RoomType = 'Deluxe' AND NOT EXISTS (
    SELECT 1 FROM BaseTariff WHERE RoomTypeId = MRoomTypes.RoomTypeId AND OccupancyCount = 3
);

PRINT 'Base tariffs seeded successfully'
