SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

BEGIN TRANSACTION;

-- Add new columns to CheckIn table
ALTER TABLE [CheckIn] ADD [TaxSlabSnapshotJson] nvarchar(max) NULL;
ALTER TABLE [CheckIn] ADD [TaxType] int NOT NULL DEFAULT 0;

-- Add new column to AdditionalCharges table
ALTER TABLE [AdditionalCharges] ADD [ApplyTax] bit NOT NULL DEFAULT CAST(1 AS bit);

-- Create TaxSlabs table
CREATE TABLE [TaxSlabs] (
    [Id] int NOT NULL IDENTITY,
    [MinAmount] decimal(10,2) NOT NULL,
    [MaxAmount] decimal(10,2) NULL,
    [CgstPercentage] decimal(5,2) NOT NULL,
    [SgstPercentage] decimal(5,2) NOT NULL,
    [IgstPercentage] decimal(5,2) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Description] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TaxSlabs] PRIMARY KEY ([Id])
);

-- Create indexes
CREATE INDEX [IX_TaxSlabs_AmountRange] ON [TaxSlabs] ([MinAmount], [MaxAmount]);
CREATE INDEX [IX_TaxSlabs_EffectiveDates] ON [TaxSlabs] ([EffectiveFrom], [EffectiveTo]);
CREATE INDEX [IX_TaxSlabs_IsActive] ON [TaxSlabs] ([IsActive]);

-- Seed initial tax slabs
INSERT INTO [TaxSlabs] (MinAmount, MaxAmount, CgstPercentage, SgstPercentage, IgstPercentage, EffectiveFrom, IsActive, Description, CreatedBy)
VALUES
(0.00, 7499.99, 2.5, 2.5, 5.0, '2020-01-01', 1, 'GST 5% for amounts up to ₹7,499.99', 'System'),
(7500.00, NULL, 9.0, 9.0, 18.0, '2020-01-01', 1, 'GST 18% for amounts ₹7,500 and above', 'System');

-- Add migration history entry
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260123074457_AddTaxSlabSystem', N'8.0.0');

COMMIT;
GO
