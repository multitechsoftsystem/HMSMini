-- Apply Unique GST Index for Company table
-- This ensures no two companies can have the same GST number

USE HMSMiniDB;
GO

BEGIN TRANSACTION;

-- Check if index already exists
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Company_GSTNumber_Unique' AND object_id = OBJECT_ID('Company'))
BEGIN
    -- Create unique filtered index on GSTNumber
    -- Only applies to non-NULL GST numbers and non-deleted companies
    CREATE UNIQUE INDEX [IX_Company_GSTNumber_Unique]
    ON [Company] ([GSTNumber])
    WHERE [GSTNumber] IS NOT NULL AND [DeletedAt] IS NULL;

    PRINT 'Created unique index on Company.GSTNumber';
END
ELSE
BEGIN
    PRINT 'Unique index IX_Company_GSTNumber_Unique already exists';
END

-- Add migration history entry if not exists
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] LIKE '%AddUniqueGSTIndex')
BEGIN
    -- Get the actual migration ID
    DECLARE @MigrationId NVARCHAR(150);
    SELECT TOP 1 @MigrationId = [MigrationId]
    FROM [__EFMigrationsHistory]
    ORDER BY [MigrationId] DESC;

    -- Insert with next timestamp
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (REPLACE(@MigrationId, LEFT(@MigrationId, 14), FORMAT(GETDATE(), 'yyyyMMddHHmmss')) + '_AddUniqueGSTIndex', N'8.0.0');

    PRINT 'Added migration history entry';
END

COMMIT TRANSACTION;
GO

PRINT 'Unique GST index applied successfully!';
GO

-- Verify no duplicate GST numbers exist
SELECT GSTNumber, COUNT(*) as Count
FROM Company
WHERE GSTNumber IS NOT NULL AND DeletedAt IS NULL
GROUP BY GSTNumber
HAVING COUNT(*) > 1;
GO

-- If duplicates found, display them
IF @@ROWCOUNT > 0
BEGIN
    PRINT 'WARNING: Duplicate GST numbers found. Please clean up before the index can work properly:';
    SELECT Id, CompanyName, GSTNumber, City
    FROM Company
    WHERE GSTNumber IN (
        SELECT GSTNumber
        FROM Company
        WHERE GSTNumber IS NOT NULL AND DeletedAt IS NULL
        GROUP BY GSTNumber
        HAVING COUNT(*) > 1
    )
    AND DeletedAt IS NULL
    ORDER BY GSTNumber, Id;
END
ELSE
BEGIN
    PRINT 'No duplicate GST numbers found - index is working correctly!';
END
GO
