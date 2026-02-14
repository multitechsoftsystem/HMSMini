-- Add Day Closing and Voucher System Migration
-- Date: 2026-01-26

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

BEGIN TRANSACTION;
GO

-- Add voucher posting columns to AdditionalCharges
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AdditionalCharges]') AND name = 'IsPostedToVoucher')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [IsPostedToVoucher] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AdditionalCharges]') AND name = 'PostedVoucherId')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [PostedVoucherId] int NULL;
END;
GO

-- Create DayClosingAudit table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DayClosingAudit')
BEGIN
    CREATE TABLE [DayClosingAudit] (
        [Id] int NOT NULL IDENTITY,
        [ClosedDate] datetime2 NOT NULL,
        [NextWorkingDate] datetime2 NOT NULL,
        [TotalActiveCheckIns] int NOT NULL DEFAULT 0,
        [TotalVouchersPosted] int NOT NULL DEFAULT 0,
        [TotalRevenuePosted] decimal(12,2) NOT NULL DEFAULT 0.0,
        [VoucherSummaryJson] nvarchar(max) NULL,
        [CheckInSummaryJson] nvarchar(max) NULL,
        [ClosingStatus] nvarchar(20) NOT NULL DEFAULT N'Completed',
        [ErrorLog] nvarchar(max) NULL,
        [ClosedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [ClosedBy] nvarchar(100) NULL,
        [DurationSeconds] int NULL,
        CONSTRAINT [PK_DayClosingAudit] PRIMARY KEY ([Id])
    );
END;
GO

-- Create SystemSettings table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemSettings')
BEGIN
    CREATE TABLE [SystemSettings] (
        [Id] int NOT NULL IDENTITY,
        [SettingKey] nvarchar(100) NOT NULL,
        [SettingValue] nvarchar(500) NOT NULL,
        [DataType] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsSystemLocked] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
    );
END;
GO

-- Create Vouchers table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vouchers')
BEGIN
    CREATE TABLE [Vouchers] (
        [Id] int NOT NULL IDENTITY,
        [VoucherNumber] nvarchar(50) NOT NULL,
        [VoucherDate] datetime2 NOT NULL,
        [PostingDate] datetime2 NOT NULL,
        [VoucherType] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Amount] decimal(10,2) NOT NULL,
        [CheckInId] int NOT NULL,
        [GuestId] int NULL,
        [RoomNumber] nvarchar(10) NOT NULL,
        [PostingStatus] nvarchar(20) NOT NULL DEFAULT N'Posted',
        [AutoPostDaily] bit NOT NULL DEFAULT CAST(0 AS bit),
        [TaxType] nvarchar(20) NULL,
        [TaxPercentage] decimal(5,2) NULL,
        [TaxableAmount] decimal(10,2) NULL,
        [AdditionalChargeId] int NULL,
        [PostedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [PostedBy] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [CancelledAt] datetime2 NULL,
        [CancelledBy] nvarchar(100) NULL,
        [CancellationReason] nvarchar(500) NULL,
        CONSTRAINT [PK_Vouchers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vouchers_AdditionalCharges_AdditionalChargeId] FOREIGN KEY ([AdditionalChargeId]) REFERENCES [AdditionalCharges] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Vouchers_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Vouchers_Guest_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [Guest] ([Id]) ON DELETE NO ACTION
    );
END;
GO

-- Seed WorkingDate setting
IF NOT EXISTS (SELECT * FROM [SystemSettings] WHERE [SettingKey] = 'WorkingDate')
BEGIN
    SET IDENTITY_INSERT [SystemSettings] ON;
    INSERT INTO [SystemSettings] ([Id], [CreatedAt], [CreatedBy], [DataType], [Description], [IsSystemLocked], [SettingKey], [SettingValue], [UpdatedAt], [UpdatedBy])
    VALUES (1, GETUTCDATE(), N'System', N'Date', N'Current business/working date for hotel operations', CAST(1 AS bit), N'WorkingDate', CONVERT(VARCHAR(10), GETDATE(), 23), NULL, NULL);
    SET IDENTITY_INSERT [SystemSettings] OFF;
END;
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AdditionalCharges_PostedVoucherId')
BEGIN
    CREATE INDEX [IX_AdditionalCharges_PostedVoucherId] ON [AdditionalCharges] ([PostedVoucherId]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DayClosingAudit_ClosedDate')
BEGIN
    CREATE UNIQUE INDEX [IX_DayClosingAudit_ClosedDate] ON [DayClosingAudit] ([ClosedDate]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SystemSettings_SettingKey')
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_SettingKey] ON [SystemSettings] ([SettingKey]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_AdditionalChargeId')
BEGIN
    CREATE INDEX [IX_Vouchers_AdditionalChargeId] ON [Vouchers] ([AdditionalChargeId]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_CheckInId')
BEGIN
    CREATE INDEX [IX_Vouchers_CheckInId] ON [Vouchers] ([CheckInId]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_GuestId')
BEGIN
    CREATE INDEX [IX_Vouchers_GuestId] ON [Vouchers] ([GuestId]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_VoucherDate')
BEGIN
    CREATE INDEX [IX_Vouchers_VoucherDate] ON [Vouchers] ([VoucherDate]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_VoucherNumber')
BEGIN
    CREATE UNIQUE INDEX [IX_Vouchers_VoucherNumber] ON [Vouchers] ([VoucherNumber]);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vouchers_VoucherType')
BEGIN
    CREATE INDEX [IX_Vouchers_VoucherType] ON [Vouchers] ([VoucherType]);
END;
GO

-- Add foreign key from AdditionalCharges to Vouchers
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AdditionalCharges_Vouchers_PostedVoucherId')
BEGIN
    ALTER TABLE [AdditionalCharges] ADD CONSTRAINT [FK_AdditionalCharges_Vouchers_PostedVoucherId]
        FOREIGN KEY ([PostedVoucherId]) REFERENCES [Vouchers] ([Id]) ON DELETE NO ACTION;
END;
GO

-- Add migration history record
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260126190554_AddDayClosingAndVoucherSystem', N'8.0.0');
END;
GO

COMMIT;
GO

PRINT 'Day Closing and Voucher System migration applied successfully!';
GO
