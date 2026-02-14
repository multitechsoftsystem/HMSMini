SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

BEGIN TRANSACTION;

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

CREATE TABLE [Invoices] (
    [Id] int NOT NULL IDENTITY,
    [InvoiceNumber] nvarchar(50) NOT NULL,
    [InvoiceDate] datetime2 NOT NULL,
    [CheckInId] int NOT NULL,
    [RoomNumber] nvarchar(10) NOT NULL,
    [GuestNames] nvarchar(500) NOT NULL,
    [CompanyName] nvarchar(200) NULL,
    [ActualCheckInDate] datetime2 NOT NULL,
    [ActualCheckOutDate] datetime2 NOT NULL,
    [TotalNights] int NOT NULL,
    [DailyChargesJson] nvarchar(max) NOT NULL,
    [AdditionalChargesJson] nvarchar(max) NULL,
    [TaxBreakdownJson] nvarchar(max) NOT NULL,
    [RoomChargesSubtotal] decimal(10,2) NOT NULL,
    [MealChargesSubtotal] decimal(10,2) NOT NULL,
    [DiscountAmount] decimal(10,2) NOT NULL,
    [AdditionalChargesSubtotal] decimal(10,2) NOT NULL,
    [SubtotalBeforeTax] decimal(10,2) NOT NULL,
    [TotalTax] decimal(10,2) NOT NULL,
    [GrandTotal] decimal(10,2) NOT NULL,
    [PaymentStatus] nvarchar(50) NOT NULL DEFAULT N'Unpaid',
    [PaymentMethod] nvarchar(50) NULL,
    [PaymentDate] datetime2 NULL,
    [PaymentNotes] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Invoices_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [TaxConfiguration] (
    [Id] int NOT NULL IDENTITY,
    [TaxType] nvarchar(50) NOT NULL,
    [TaxPercentage] decimal(5,2) NOT NULL,
    [ApplicableOn] nvarchar(50) NOT NULL DEFAULT N'All',
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TaxConfiguration] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_AdditionalCharges_ChargeDate] ON [AdditionalCharges] ([ChargeDate]);
CREATE INDEX [IX_AdditionalCharges_CheckInId] ON [AdditionalCharges] ([CheckInId]);
CREATE UNIQUE INDEX [IX_Invoices_CheckInId] ON [Invoices] ([CheckInId]);
CREATE INDEX [IX_Invoices_InvoiceDate] ON [Invoices] ([InvoiceDate]);
CREATE UNIQUE INDEX [IX_Invoices_InvoiceNumber] ON [Invoices] ([InvoiceNumber]);
CREATE INDEX [IX_TaxConfiguration_EffectiveDates] ON [TaxConfiguration] ([EffectiveFrom], [EffectiveTo]);
CREATE INDEX [IX_TaxConfiguration_IsActive] ON [TaxConfiguration] ([IsActive]);

-- Insert default tax configuration (CGST + SGST)
INSERT INTO [TaxConfiguration] ([TaxType], [TaxPercentage], [ApplicableOn], [EffectiveFrom], [IsActive], [CreatedAt], [CreatedBy])
VALUES
    (N'CGST', 9.00, N'All', '2020-01-01', 1, GETUTCDATE(), N'System'),
    (N'SGST', 9.00, N'All', '2020-01-01', 1, GETUTCDATE(), N'System');

-- Mark migration as applied
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260122000825_AddBillingEntities', N'8.0.0');

COMMIT TRANSACTION;
