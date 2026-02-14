-- Complete Tax Slab Migration Script
-- This script creates the TaxSlabs table, adds columns to existing tables, and seeds initial data

USE HMSMiniDB;
GO

BEGIN TRANSACTION;

-- 1. Create TaxSlabs table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaxSlabs')
BEGIN
    CREATE TABLE [TaxSlabs] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MinAmount] decimal(10,2) NOT NULL,
        [MaxAmount] decimal(10,2) NULL,
        [CgstPercentage] decimal(5,2) NOT NULL,
        [SgstPercentage] decimal(5,2) NOT NULL,
        [IgstPercentage] decimal(5,2) NOT NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [Description] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_TaxSlabs] PRIMARY KEY ([Id])
    );
    PRINT 'TaxSlabs table created';
END
ELSE
BEGIN
    PRINT 'TaxSlabs table already exists';
END

-- 2. Add new columns to CheckIn table (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CheckIn') AND name = 'TaxSlabSnapshotJson')
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxSlabSnapshotJson] nvarchar(max) NULL;
    PRINT 'Added TaxSlabSnapshotJson column to CheckIn';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CheckIn') AND name = 'TaxType')
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxType] int NOT NULL DEFAULT 0;
    PRINT 'Added TaxType column to CheckIn';
END

-- 3. Add new column to AdditionalCharges table (only if it doesn't exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AdditionalCharges') AND name = 'ApplyTax')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [ApplyTax] bit NOT NULL DEFAULT CAST(1 AS bit);
    PRINT 'Added ApplyTax column to AdditionalCharges';
END

-- 4. Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_AmountRange' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_AmountRange] ON [TaxSlabs] ([MinAmount], [MaxAmount]);
    PRINT 'Created IX_TaxSlabs_AmountRange index';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_EffectiveDates' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_EffectiveDates] ON [TaxSlabs] ([EffectiveFrom], [EffectiveTo]);
    PRINT 'Created IX_TaxSlabs_EffectiveDates index';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_IsActive' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_IsActive] ON [TaxSlabs] ([IsActive]);
    PRINT 'Created IX_TaxSlabs_IsActive index';
END

-- 5. Seed initial tax slabs if table is empty
IF NOT EXISTS (SELECT * FROM [TaxSlabs])
BEGIN
    INSERT INTO [TaxSlabs] ([MinAmount], [MaxAmount], [CgstPercentage], [SgstPercentage], [IgstPercentage], [EffectiveFrom], [IsActive], [Description], [CreatedBy], [CreatedAt])
    VALUES
    (0.00, 7499.99, 2.5, 2.5, 5.0, '2020-01-01', 1, 'GST 5% for amounts up to ₹7,499.99', 'System', GETUTCDATE()),
    (7500.00, NULL, 9.0, 9.0, 18.0, '2020-01-01', 1, 'GST 18% for amounts ₹7,500 and above', 'System', GETUTCDATE());
    PRINT 'Seeded 2 tax slabs';
END
ELSE
BEGIN
    PRINT 'Tax slabs already exist - skipping seed';
END

-- 6. Add migration history entry if not exists
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123074457_AddTaxSlabSystem', N'8.0.0');
    PRINT 'Added migration history entry';
END

COMMIT TRANSACTION;
GO

PRINT 'Tax slab migration completed successfully!';
GO

-- Verify the data
SELECT * FROM [TaxSlabs];
GO
