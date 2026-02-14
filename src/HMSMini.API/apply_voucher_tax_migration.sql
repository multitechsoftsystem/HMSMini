-- Apply Voucher Tax Configuration Migration
-- This script creates the VoucherTaxConfigurations table and seeds common voucher types

USE HMSMiniDB;
GO

BEGIN TRANSACTION;

-- 1. Create VoucherTaxConfigurations table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VoucherTaxConfigurations')
BEGIN
    CREATE TABLE [VoucherTaxConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [VoucherType] nvarchar(100) NOT NULL,
        [CgstPercentage] decimal(5,2) NOT NULL,
        [SgstPercentage] decimal(5,2) NOT NULL,
        [IgstPercentage] decimal(5,2) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [Description] nvarchar(500) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_VoucherTaxConfigurations] PRIMARY KEY ([Id])
    );
    PRINT 'VoucherTaxConfigurations table created';
END
ELSE
BEGIN
    PRINT 'VoucherTaxConfigurations table already exists';
END

-- 2. Add VoucherTaxConfigId column to AdditionalCharges table (only if it doesn't exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AdditionalCharges') AND name = 'VoucherTaxConfigId')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [VoucherTaxConfigId] int NULL;
    PRINT 'Added VoucherTaxConfigId column to AdditionalCharges';
END

-- 3. Create foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AdditionalCharges_VoucherTaxConfigurations_VoucherTaxConfigId')
BEGIN
    ALTER TABLE [AdditionalCharges]
    ADD CONSTRAINT [FK_AdditionalCharges_VoucherTaxConfigurations_VoucherTaxConfigId]
    FOREIGN KEY ([VoucherTaxConfigId])
    REFERENCES [VoucherTaxConfigurations] ([Id])
    ON DELETE SET NULL;
    PRINT 'Created foreign key constraint';
END

-- 4. Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VoucherTaxConfigurations_VoucherType' AND object_id = OBJECT_ID('VoucherTaxConfigurations'))
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_VoucherType] ON [VoucherTaxConfigurations] ([VoucherType]);
    PRINT 'Created IX_VoucherTaxConfigurations_VoucherType index';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VoucherTaxConfigurations_IsActive' AND object_id = OBJECT_ID('VoucherTaxConfigurations'))
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_IsActive] ON [VoucherTaxConfigurations] ([IsActive]);
    PRINT 'Created IX_VoucherTaxConfigurations_IsActive index';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VoucherTaxConfigurations_DisplayOrder' AND object_id = OBJECT_ID('VoucherTaxConfigurations'))
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_DisplayOrder] ON [VoucherTaxConfigurations] ([DisplayOrder]);
    PRINT 'Created IX_VoucherTaxConfigurations_DisplayOrder index';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AdditionalCharges_VoucherTaxConfigId' AND object_id = OBJECT_ID('AdditionalCharges'))
BEGIN
    CREATE INDEX [IX_AdditionalCharges_VoucherTaxConfigId] ON [AdditionalCharges] ([VoucherTaxConfigId]);
    PRINT 'Created IX_AdditionalCharges_VoucherTaxConfigId index';
END

-- 5. Seed common voucher tax configurations if table is empty
IF NOT EXISTS (SELECT * FROM [VoucherTaxConfigurations])
BEGIN
    INSERT INTO [VoucherTaxConfigurations]
    ([VoucherType], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive], [Description], [DisplayOrder], [CreatedBy], [CreatedAt])
    VALUES
    ('Room Service', 2.5, 2.5, 5.0, 1, 'GST 5% for room service (food items)', 1, 'System', GETUTCDATE()),
    ('Restaurant', 2.5, 2.5, 5.0, 1, 'GST 5% for restaurant charges (food items)', 2, 'System', GETUTCDATE()),
    ('Minibar', 9.0, 9.0, 18.0, 1, 'GST 18% for minibar items (beverages and snacks)', 3, 'System', GETUTCDATE()),
    ('Laundry', 9.0, 9.0, 18.0, 1, 'GST 18% for laundry services', 4, 'System', GETUTCDATE()),
    ('Telephone', 9.0, 9.0, 18.0, 1, 'GST 18% for telephone charges', 5, 'System', GETUTCDATE()),
    ('Internet', 9.0, 9.0, 18.0, 1, 'GST 18% for internet services', 6, 'System', GETUTCDATE()),
    ('Parking', 9.0, 9.0, 18.0, 1, 'GST 18% for parking charges', 7, 'System', GETUTCDATE()),
    ('Spa/Massage', 9.0, 9.0, 18.0, 1, 'GST 18% for spa and massage services', 8, 'System', GETUTCDATE()),
    ('Transportation', 2.5, 2.5, 5.0, 1, 'GST 5% for transportation services', 9, 'System', GETUTCDATE()),
    ('Other', 9.0, 9.0, 18.0, 1, 'GST 18% for miscellaneous charges', 10, 'System', GETUTCDATE());

    PRINT 'Seeded 10 voucher tax configurations';
END
ELSE
BEGIN
    PRINT 'Voucher tax configurations already exist - skipping seed';
END

COMMIT TRANSACTION;
GO

PRINT 'Voucher tax configuration migration completed successfully!';
GO

-- Verify the data
SELECT * FROM [VoucherTaxConfigurations] ORDER BY [DisplayOrder];
GO
