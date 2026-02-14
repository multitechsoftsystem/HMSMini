-- Fix Duplicate GST Numbers and Add Unique Index
USE HMSMiniDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Step 1: Check for duplicate GST numbers
PRINT '=== Checking for duplicate GST numbers ===';
SELECT GSTNumber, COUNT(*) as Count
FROM Company
WHERE GSTNumber IS NOT NULL AND DeletedAt IS NULL
GROUP BY GSTNumber
HAVING COUNT(*) > 1;
GO

-- Step 2: Display companies with duplicate GST numbers
IF EXISTS (
    SELECT 1
    FROM Company
    WHERE GSTNumber IS NOT NULL AND DeletedAt IS NULL
    GROUP BY GSTNumber
    HAVING COUNT(*) > 1
)
BEGIN
    PRINT 'WARNING: Duplicate GST numbers found:';
    SELECT Id, CompanyName, GSTNumber, City, ContactPerson, ContactNumber
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

    PRINT '';
    PRINT 'To fix duplicates, you can either:';
    PRINT '1. Update one of the companies to have a different GST number';
    PRINT '2. Delete/soft-delete one of the duplicate companies';
    PRINT '';
    PRINT 'Example to clear GST from duplicate (keeping first occurrence):';
    PRINT 'UPDATE Company SET GSTNumber = NULL WHERE Id = 3; -- Change ID as needed';
    PRINT '';
    PRINT 'Run this script again after fixing duplicates to create the unique index.';
END
ELSE
BEGIN
    PRINT 'No duplicate GST numbers found. Creating unique index...';

    BEGIN TRANSACTION;

    -- Drop existing index if it exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Company_GSTNumber_Unique' AND object_id = OBJECT_ID('Company'))
    BEGIN
        DROP INDEX [IX_Company_GSTNumber_Unique] ON [Company];
        PRINT 'Dropped existing index';
    END

    -- Create unique filtered index on GSTNumber
    CREATE UNIQUE INDEX [IX_Company_GSTNumber_Unique]
    ON [Company] ([GSTNumber])
    WHERE [GSTNumber] IS NOT NULL AND [DeletedAt] IS NULL;

    PRINT 'Created unique index on Company.GSTNumber';

    COMMIT TRANSACTION;

    PRINT '';
    PRINT 'SUCCESS: Unique GST index created successfully!';
    PRINT 'Companies can no longer be created with duplicate GST numbers.';
END
GO
