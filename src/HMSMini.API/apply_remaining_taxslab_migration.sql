SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

BEGIN TRANSACTION;

-- Add new columns to CheckIn table (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CheckIn') AND name = 'TaxSlabSnapshotJson')
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxSlabSnapshotJson] nvarchar(max) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CheckIn') AND name = 'TaxType')
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxType] int NOT NULL DEFAULT 0;
END

-- Add new column to AdditionalCharges table (only if it doesn't exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AdditionalCharges') AND name = 'ApplyTax')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [ApplyTax] bit NOT NULL DEFAULT CAST(1 AS bit);
END

-- Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_AmountRange' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_AmountRange] ON [TaxSlabs] ([MinAmount], [MaxAmount]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_EffectiveDates' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_EffectiveDates] ON [TaxSlabs] ([EffectiveFrom], [EffectiveTo]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaxSlabs_IsActive' AND object_id = OBJECT_ID('TaxSlabs'))
BEGIN
    CREATE INDEX [IX_TaxSlabs_IsActive] ON [TaxSlabs] ([IsActive]);
END

-- Seed initial tax slabs if table is empty
IF NOT EXISTS (SELECT * FROM [TaxSlabs])
BEGIN
    INSERT INTO [TaxSlabs] (MinAmount, MaxAmount, CgstPercentage, SgstPercentage, IgstPercentage, EffectiveFrom, IsActive, Description, CreatedBy)
    VALUES
    (0.00, 7499.99, 2.5, 2.5, 5.0, '2020-01-01', 1, 'GST 5% for amounts up to ₹7,499.99', 'System'),
    (7500.00, NULL, 9.0, 9.0, 18.0, '2020-01-01', 1, 'GST 18% for amounts ₹7,500 and above', 'System');
END

-- Add migration history entry if not exists
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123074457_AddTaxSlabSystem', N'8.0.0');
END

COMMIT;
GO

PRINT 'Tax slab migration applied successfully';
GO
