SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='AdditionalCharges')
BEGIN
    CREATE TABLE [AdditionalCharges] (
        [Id] int NOT NULL IDENTITY,
        [CheckInId] int NOT NULL,
        [ChargeDate] datetime2 NOT NULL,
        [ChargeType] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [Quantity] int NOT NULL DEFAULT 1,
        [TotalAmount] AS [Amount] * [Quantity] PERSISTED,
        [AddedBy] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_AdditionalCharges] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdditionalCharges_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_AdditionalCharges_ChargeDate] ON [AdditionalCharges] ([ChargeDate]);
    CREATE INDEX [IX_AdditionalCharges_CheckInId] ON [AdditionalCharges] ([CheckInId]);

    PRINT 'AdditionalCharges table created successfully';
END
ELSE
BEGIN
    PRINT 'AdditionalCharges table already exists';
END
GO

-- Insert default tax configuration if not exists
IF NOT EXISTS (SELECT * FROM TaxConfiguration WHERE TaxType = 'CGST')
BEGIN
    INSERT INTO [TaxConfiguration] ([TaxType], [TaxPercentage], [ApplicableOn], [EffectiveFrom], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (N'CGST', 9.00, N'All', '2020-01-01', 1, GETUTCDATE(), N'System');
    PRINT 'CGST tax configuration inserted';
END

IF NOT EXISTS (SELECT * FROM TaxConfiguration WHERE TaxType = 'SGST')
BEGIN
    INSERT INTO [TaxConfiguration] ([TaxType], [TaxPercentage], [ApplicableOn], [EffectiveFrom], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (N'SGST', 9.00, N'All', '2020-01-01', 1, GETUTCDATE(), N'System');
    PRINT 'SGST tax configuration inserted';
END
GO

-- Mark migration as applied if not already
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = N'20260122000825_AddBillingEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122000825_AddBillingEntities', N'8.0.0');
    PRINT 'Migration marked as applied';
END
ELSE
BEGIN
    PRINT 'Migration already applied';
END
GO
