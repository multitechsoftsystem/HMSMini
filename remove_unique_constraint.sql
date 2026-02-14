-- Remove unique constraint from Guest table to allow historical records
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Guest_CheckInId_GuestNumber' AND is_unique = 1)
BEGIN
    DROP INDEX IX_Guest_CheckInId_GuestNumber ON Guest;
    CREATE INDEX IX_Guest_CheckInId_GuestNumber ON Guest (CheckInId, GuestNumber);
    PRINT 'Unique constraint removed from Guest table';
END
ELSE
BEGIN
    PRINT 'Unique constraint already removed or does not exist';
END
