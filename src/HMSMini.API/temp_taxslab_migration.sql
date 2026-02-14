BEGIN TRANSACTION;
GO

ALTER TABLE [CheckIn] ADD [TaxSlabSnapshotJson] nvarchar(max) NULL;
GO

ALTER TABLE [CheckIn] ADD [TaxType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [AdditionalCharges] ADD [ApplyTax] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

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
GO

CREATE INDEX [IX_TaxSlabs_AmountRange] ON [TaxSlabs] ([MinAmount], [MaxAmount]);
GO

CREATE INDEX [IX_TaxSlabs_EffectiveDates] ON [TaxSlabs] ([EffectiveFrom], [EffectiveTo]);
GO

CREATE INDEX [IX_TaxSlabs_IsActive] ON [TaxSlabs] ([IsActive]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260123074457_AddTaxSlabSystem', N'8.0.0');
GO

COMMIT;
GO

