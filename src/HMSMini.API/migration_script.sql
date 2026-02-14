BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reservations]') AND [c].[name] = N'RoomId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Reservations] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Reservations] ALTER COLUMN [RoomId] int NULL;
GO

-- Add RoomTypeId column as nullable first
ALTER TABLE [Reservations] ADD [RoomTypeId] int NULL;
GO

-- Update existing reservations to set RoomTypeId based on their Room's RoomTypeId
UPDATE r
SET r.RoomTypeId = rn.RoomTypeId
FROM [Reservations] r
INNER JOIN [Rooms] rn ON r.RoomId = rn.RoomId
WHERE r.RoomTypeId IS NULL;
GO

-- Now make RoomTypeId NOT NULL with a default for future inserts
ALTER TABLE [Reservations] ALTER COLUMN [RoomTypeId] int NOT NULL;
GO

CREATE INDEX [IX_Reservations_RoomTypeId] ON [Reservations] ([RoomTypeId]);
GO

ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251231064508_UpdateReservationToUseRoomType', N'8.0.0');
GO

COMMIT;
GO

