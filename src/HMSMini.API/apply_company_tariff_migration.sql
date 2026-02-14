-- Migration: AddCompanyAndTariffSystem
-- Add columns to Reservations table
ALTER TABLE [Reservations] ADD [CompanyId] int NULL;
ALTER TABLE [Reservations] ADD [EstimatedAmount] decimal(10,2) NULL;

-- Add columns to CheckIn table
ALTER TABLE [CheckIn] ADD [CompanyId] int NULL;
ALTER TABLE [CheckIn] ADD [DiscountPercentage] decimal(5,2) NOT NULL DEFAULT 0;
ALTER TABLE [CheckIn] ADD [FinalAmount] decimal(10,2) NULL;
ALTER TABLE [CheckIn] ADD [TariffApplied] decimal(10,2) NULL;

-- Create BaseTariff table
CREATE TABLE [BaseTariff] (
    [Id] int NOT NULL IDENTITY(1,1),
    [RoomTypeId] int NOT NULL,
    [OccupancyCount] int NOT NULL,
    [RatePerNight] decimal(10,2) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_BaseTariff] PRIMARY KEY ([Id])
);

-- Create Company table
CREATE TABLE [Company] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CompanyName] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NULL,
    [City] nvarchar(100) NULL,
    [State] nvarchar(100) NULL,
    [Country] nvarchar(100) NULL,
    [GSTNumber] nvarchar(50) NULL,
    [ContactPerson] nvarchar(200) NULL,
    [Designation] nvarchar(100) NULL,
    [Email] nvarchar(255) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DiscountPercentage] decimal(5,2) NOT NULL DEFAULT 0,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
);

-- Create CompanyTariff table
CREATE TABLE [CompanyTariff] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CompanyId] int NOT NULL,
    [RoomTypeId] int NOT NULL,
    [OccupancyCount] int NOT NULL,
    [RatePerNight] decimal(10,2) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_CompanyTariff] PRIMARY KEY ([Id])
);

-- Create indexes
CREATE INDEX [IX_Reservations_CompanyId] ON [Reservations] ([CompanyId]);
CREATE INDEX [IX_CheckIn_CompanyId] ON [CheckIn] ([CompanyId]);
CREATE INDEX [IX_BaseTariff_EffectiveFrom_EffectiveTo] ON [BaseTariff] ([EffectiveFrom], [EffectiveTo]);
CREATE INDEX [IX_BaseTariff_IsActive] ON [BaseTariff] ([IsActive]);
CREATE INDEX [IX_BaseTariff_RoomTypeId] ON [BaseTariff] ([RoomTypeId]);
CREATE UNIQUE INDEX [IX_BaseTariff_RoomTypeId_OccupancyCount_EffectiveFrom] ON [BaseTariff] ([RoomTypeId], [OccupancyCount], [EffectiveFrom]);
CREATE INDEX [IX_Company_CompanyName] ON [Company] ([CompanyName]);
CREATE INDEX [IX_Company_DeletedAt] ON [Company] ([DeletedAt]);
CREATE INDEX [IX_Company_IsActive] ON [Company] ([IsActive]);
CREATE INDEX [IX_CompanyTariff_CompanyId] ON [CompanyTariff] ([CompanyId]);
CREATE UNIQUE INDEX [IX_CompanyTariff_CompanyId_RoomTypeId_OccupancyCount_EffectiveFrom] ON [CompanyTariff] ([CompanyId], [RoomTypeId], [OccupancyCount], [EffectiveFrom]);
CREATE INDEX [IX_CompanyTariff_EffectiveFrom_EffectiveTo] ON [CompanyTariff] ([EffectiveFrom], [EffectiveTo]);
CREATE INDEX [IX_CompanyTariff_IsActive] ON [CompanyTariff] ([IsActive]);
CREATE INDEX [IX_CompanyTariff_RoomTypeId] ON [CompanyTariff] ([RoomTypeId]);

-- Add foreign keys
ALTER TABLE [BaseTariff] ADD CONSTRAINT [FK_BaseTariff_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION;
ALTER TABLE [CompanyTariff] ADD CONSTRAINT [FK_CompanyTariff_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [CompanyTariff] ADD CONSTRAINT [FK_CompanyTariff_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION;
ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION;

-- Update migrations history
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260121015409_AddCompanyAndTariffSystem', N'8.0.0');
