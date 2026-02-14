IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE TABLE [MRoomTypes] (
        [RoomTypeId] int NOT NULL IDENTITY,
        [RoomType] nvarchar(50) NOT NULL,
        [RoomDescription] nvarchar(500) NULL,
        CONSTRAINT [PK_MRoomTypes] PRIMARY KEY ([RoomTypeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE TABLE [RoomNo] (
        [RoomId] int NOT NULL IDENTITY,
        [RoomNumber] nvarchar(20) NOT NULL,
        [RoomTypeId] int NOT NULL,
        [RoomStatus] int NOT NULL,
        [RoomStatusFromDate] datetime2 NULL,
        [RoomStatusToDate] datetime2 NULL,
        CONSTRAINT [PK_RoomNo] PRIMARY KEY ([RoomId]),
        CONSTRAINT [FK_RoomNo_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE TABLE [CheckIn] (
        [Id] int NOT NULL IDENTITY,
        [RoomId] int NOT NULL,
        [CheckInDate] datetime2 NOT NULL,
        [CheckOutDate] datetime2 NOT NULL,
        [ActualCheckOutDate] datetime2 NULL,
        [Pax] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CheckIn] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CheckIn_RoomNo_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [RoomNo] ([RoomId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE TABLE [Guest] (
        [Id] int NOT NULL IDENTITY,
        [CheckInId] int NOT NULL,
        [GuestNumber] int NOT NULL,
        [GuestName] nvarchar(200) NOT NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NULL,
        [State] nvarchar(100) NULL,
        [Country] nvarchar(100) NULL,
        [MobileNo] nvarchar(20) NULL,
        [Photo1Path] nvarchar(500) NULL,
        [Photo2Path] nvarchar(500) NULL,
        CONSTRAINT [PK_Guest] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Guest_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CheckIn_CheckInDate] ON [CheckIn] ([CheckInDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CheckIn_CheckOutDate] ON [CheckIn] ([CheckOutDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CheckIn_RoomId_Status] ON [CheckIn] ([RoomId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CheckIn_Status] ON [CheckIn] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Guest_CheckInId_GuestNumber] ON [Guest] ([CheckInId], [GuestNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MRoomTypes_RoomType] ON [MRoomTypes] ([RoomType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RoomNo_RoomNumber] ON [RoomNo] ([RoomNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoomNo_RoomStatus] ON [RoomNo] ([RoomStatus]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoomNo_RoomStatus_RoomStatusFromDate_RoomStatusToDate] ON [RoomNo] ([RoomStatus], [RoomStatusFromDate], [RoomStatusToDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoomNo_RoomTypeId] ON [RoomNo] ([RoomTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226134936_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251226134936_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [CreatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [DeletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [DeletedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [UpdatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [RoomNo] ADD [UpdatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [CreatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [DeletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [DeletedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [UpdatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [MRoomTypes] ADD [UpdatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [CreatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [DeletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [DeletedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [PanOrAadharNo] nvarchar(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [UpdatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [Guest] ADD [UpdatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [ActualCheckInDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [CreatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [DeletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [DeletedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [RegistrationNo] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [Remarks] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [UpdatedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227084202_AddAuditFieldsAndNewColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251227084202_AddAuditFieldsAndNewColumns', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227122445_AddUserAuthentication'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(100) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(200) NULL,
        [Role] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [LastLoginAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227122445_AddUserAuthentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227122445_AddUserAuthentication'
)
BEGIN
    CREATE INDEX [IX_Users_IsActive] ON [Users] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227122445_AddUserAuthentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227122445_AddUserAuthentication'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251227122445_AddUserAuthentication', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE TABLE [Reservations] (
        [Id] int NOT NULL IDENTITY,
        [ReservationNumber] nvarchar(50) NOT NULL,
        [RoomId] int NOT NULL,
        [CheckInDate] datetime2 NOT NULL,
        [CheckOutDate] datetime2 NOT NULL,
        [NumberOfGuests] int NOT NULL,
        [GuestName] nvarchar(200) NOT NULL,
        [GuestEmail] nvarchar(255) NULL,
        [GuestMobile] nvarchar(20) NOT NULL,
        [SpecialRequests] nvarchar(1000) NULL,
        [Status] nvarchar(450) NOT NULL,
        [CheckInId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] int NULL,
        [UpdatedBy] int NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reservations_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Reservations_RoomNo_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [RoomNo] ([RoomId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE INDEX [IX_Reservations_CheckInDate] ON [Reservations] ([CheckInDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE INDEX [IX_Reservations_CheckInId] ON [Reservations] ([CheckInId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reservations_ReservationNumber] ON [Reservations] ([ReservationNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE INDEX [IX_Reservations_RoomId_CheckInDate_CheckOutDate] ON [Reservations] ([RoomId], [CheckInDate], [CheckOutDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    CREATE INDEX [IX_Reservations_Status] ON [Reservations] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251227190847_AddReservationSystem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251227190847_AddReservationSystem', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231064508_UpdateReservationToUseRoomType'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reservations]') AND [c].[name] = N'RoomId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Reservations] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Reservations] ALTER COLUMN [RoomId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231064508_UpdateReservationToUseRoomType'
)
BEGIN
    ALTER TABLE [Reservations] ADD [RoomTypeId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231064508_UpdateReservationToUseRoomType'
)
BEGIN
    CREATE INDEX [IX_Reservations_RoomTypeId] ON [Reservations] ([RoomTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231064508_UpdateReservationToUseRoomType'
)
BEGIN
    ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231064508_UpdateReservationToUseRoomType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251231064508_UpdateReservationToUseRoomType', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231190803_AddGuestCheckInOutTracking'
)
BEGIN
    ALTER TABLE [Guest] ADD [ActualCheckInDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231190803_AddGuestCheckInOutTracking'
)
BEGIN
    ALTER TABLE [Guest] ADD [ActualCheckOutDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231190803_AddGuestCheckInOutTracking'
)
BEGIN
    ALTER TABLE [Guest] ADD [Status] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251231190803_AddGuestCheckInOutTracking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251231190803_AddGuestCheckInOutTracking', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101022913_RemoveGuestNumberUniqueConstraint'
)
BEGIN
    DROP INDEX [IX_Guest_CheckInId_GuestNumber] ON [Guest];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101022913_RemoveGuestNumberUniqueConstraint'
)
BEGIN
    CREATE INDEX [IX_Guest_CheckInId_GuestNumber] ON [Guest] ([CheckInId], [GuestNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101022913_RemoveGuestNumberUniqueConstraint'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260101022913_RemoveGuestNumberUniqueConstraint', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [Reservations] ADD [CompanyId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [Reservations] ADD [EstimatedAmount] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [CompanyId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [DiscountPercentage] decimal(5,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [FinalAmount] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [TariffApplied] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE TABLE [BaseTariff] (
        [Id] int NOT NULL IDENTITY,
        [RoomTypeId] int NOT NULL,
        [OccupancyCount] int NOT NULL,
        [RatePerNight] decimal(10,2) NOT NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_BaseTariff] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BaseTariff_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE TABLE [Company] (
        [Id] int NOT NULL IDENTITY,
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
        [DiscountPercentage] decimal(5,2) NOT NULL DEFAULT 0.0,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE TABLE [CompanyTariff] (
        [Id] int NOT NULL IDENTITY,
        [CompanyId] int NOT NULL,
        [RoomTypeId] int NOT NULL,
        [OccupancyCount] int NOT NULL,
        [RatePerNight] decimal(10,2) NOT NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_CompanyTariff] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyTariff_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompanyTariff_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_Reservations_CompanyId] ON [Reservations] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_CheckIn_CompanyId] ON [CheckIn] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_BaseTariff_EffectiveFrom_EffectiveTo] ON [BaseTariff] ([EffectiveFrom], [EffectiveTo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_BaseTariff_IsActive] ON [BaseTariff] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_BaseTariff_RoomTypeId] ON [BaseTariff] ([RoomTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BaseTariff_RoomTypeId_OccupancyCount_EffectiveFrom] ON [BaseTariff] ([RoomTypeId], [OccupancyCount], [EffectiveFrom]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_Company_CompanyName] ON [Company] ([CompanyName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_Company_DeletedAt] ON [Company] ([DeletedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_Company_IsActive] ON [Company] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_CompanyTariff_CompanyId] ON [CompanyTariff] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CompanyTariff_CompanyId_RoomTypeId_OccupancyCount_EffectiveFrom] ON [CompanyTariff] ([CompanyId], [RoomTypeId], [OccupancyCount], [EffectiveFrom]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_CompanyTariff_EffectiveFrom_EffectiveTo] ON [CompanyTariff] ([EffectiveFrom], [EffectiveTo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_CompanyTariff_IsActive] ON [CompanyTariff] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    CREATE INDEX [IX_CompanyTariff_RoomTypeId] ON [CompanyTariff] ([RoomTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121015409_AddCompanyAndTariffSystem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260121015409_AddCompanyAndTariffSystem', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reservations]') AND [c].[name] = N'EstimatedAmount');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Reservations] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Reservations] ALTER COLUMN [EstimatedAmount] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    ALTER TABLE [Reservations] ADD [BusinessSourceId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [BusinessSourceId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE TABLE [MBusinessSources] (
        [BusinessSourceId] int NOT NULL IDENTITY,
        [SourceName] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_MBusinessSources] PRIMARY KEY ([BusinessSourceId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE INDEX [IX_Reservations_BusinessSourceId] ON [Reservations] ([BusinessSourceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE INDEX [IX_CheckIn_BusinessSourceId] ON [CheckIn] ([BusinessSourceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE INDEX [IX_MBusinessSources_DeletedAt] ON [MBusinessSources] ([DeletedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE INDEX [IX_MBusinessSources_IsActive] ON [MBusinessSources] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MBusinessSources_SourceName] ON [MBusinessSources] ([SourceName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_MBusinessSources_BusinessSourceId] FOREIGN KEY ([BusinessSourceId]) REFERENCES [MBusinessSources] ([BusinessSourceId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MBusinessSources_BusinessSourceId] FOREIGN KEY ([BusinessSourceId]) REFERENCES [MBusinessSources] ([BusinessSourceId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121103024_AddBusinessSourceTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260121103024_AddBusinessSourceTables', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    ALTER TABLE [Reservations] ADD [MealPlanId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [MealPlanId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [MealPlanRate] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE TABLE [MMealPlans] (
        [MealPlanId] int NOT NULL IDENTITY,
        [PlanCode] nvarchar(10) NOT NULL,
        [PlanName] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_MMealPlans] PRIMARY KEY ([MealPlanId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE TABLE [MealPlanRates] (
        [Id] int NOT NULL IDENTITY,
        [MealPlanId] int NOT NULL,
        [RoomTypeId] int NOT NULL,
        [RatePerPersonPerNight] decimal(10,2) NOT NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_MealPlanRates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MealPlanRates_MMealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MMealPlans] ([MealPlanId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MealPlanRates_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_Reservations_MealPlanId] ON [Reservations] ([MealPlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_CheckIn_MealPlanId] ON [CheckIn] ([MealPlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MealPlanRates_DeletedAt] ON [MealPlanRates] ([DeletedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MealPlanRates_IsActive] ON [MealPlanRates] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MealPlanRates_MealPlan_RoomType_EffectiveFrom] ON [MealPlanRates] ([MealPlanId], [RoomTypeId], [EffectiveFrom]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MealPlanRates_RoomTypeId] ON [MealPlanRates] ([RoomTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MMealPlans_DeletedAt] ON [MMealPlans] ([DeletedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE INDEX [IX_MMealPlans_IsActive] ON [MMealPlans] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MMealPlans_PlanCode] ON [MMealPlans] ([PlanCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_MMealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MMealPlans] ([MealPlanId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MMealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MMealPlans] ([MealPlanId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121122456_AddMealPlanTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260121122456_AddMealPlanTables', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
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
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
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
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
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
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE INDEX [IX_AdditionalCharges_ChargeDate] ON [AdditionalCharges] ([ChargeDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE INDEX [IX_AdditionalCharges_CheckInId] ON [AdditionalCharges] ([CheckInId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Invoices_CheckInId] ON [Invoices] ([CheckInId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE INDEX [IX_Invoices_InvoiceDate] ON [Invoices] ([InvoiceDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Invoices_InvoiceNumber] ON [Invoices] ([InvoiceNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE INDEX [IX_TaxConfiguration_EffectiveDates] ON [TaxConfiguration] ([EffectiveFrom], [EffectiveTo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    CREATE INDEX [IX_TaxConfiguration_IsActive] ON [TaxConfiguration] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000825_AddBillingEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122000825_AddBillingEntities', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxSlabSnapshotJson] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [TaxType] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [ApplyTax] bit NOT NULL DEFAULT CAST(1 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
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
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    CREATE INDEX [IX_TaxSlabs_AmountRange] ON [TaxSlabs] ([MinAmount], [MaxAmount]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    CREATE INDEX [IX_TaxSlabs_EffectiveDates] ON [TaxSlabs] ([EffectiveFrom], [EffectiveTo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    CREATE INDEX [IX_TaxSlabs_IsActive] ON [TaxSlabs] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123074457_AddTaxSlabSystem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123074457_AddTaxSlabSystem', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [VoucherTaxConfigId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    CREATE TABLE [VoucherTaxConfigurations] (
        [Id] int NOT NULL IDENTITY,
        [VoucherType] nvarchar(100) NOT NULL,
        [CgstPercentage] decimal(5,2) NOT NULL,
        [SgstPercentage] decimal(5,2) NOT NULL,
        [IgstPercentage] decimal(5,2) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Description] nvarchar(500) NULL,
        [DisplayOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(100) NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_VoucherTaxConfigurations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    CREATE INDEX [IX_AdditionalCharges_VoucherTaxConfigId] ON [AdditionalCharges] ([VoucherTaxConfigId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_DisplayOrder] ON [VoucherTaxConfigurations] ([DisplayOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_IsActive] ON [VoucherTaxConfigurations] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    CREATE INDEX [IX_VoucherTaxConfigurations_VoucherType] ON [VoucherTaxConfigurations] ([VoucherType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD CONSTRAINT [FK_AdditionalCharges_VoucherTaxConfigurations_VoucherTaxConfigId] FOREIGN KEY ([VoucherTaxConfigId]) REFERENCES [VoucherTaxConfigurations] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123093018_AddVoucherTaxConfiguration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123093018_AddVoucherTaxConfiguration', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123121503_AddUniqueGSTIndex'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Company_GSTNumber_Unique] ON [Company] ([GSTNumber]) WHERE [GSTNumber] IS NOT NULL AND [DeletedAt] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123121503_AddUniqueGSTIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123121503_AddUniqueGSTIndex', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    ALTER TABLE [CheckIn] ADD [GuestTypeId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    CREATE TABLE [MGuestTypes] (
        [GuestTypeId] int NOT NULL IDENTITY,
        [TypeName] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DisplayOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_MGuestTypes] PRIMARY KEY ([GuestTypeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'GuestTypeId', N'CreatedAt', N'DeletedAt', N'Description', N'DisplayOrder', N'IsActive', N'TypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[MGuestTypes]'))
        SET IDENTITY_INSERT [MGuestTypes] ON;
    EXEC(N'INSERT INTO [MGuestTypes] ([GuestTypeId], [CreatedAt], [DeletedAt], [Description], [DisplayOrder], [IsActive], [TypeName], [UpdatedAt])
    VALUES (1, ''2026-01-26T11:40:13.6886836Z'', NULL, N''Regular paying guest'', 1, CAST(1 AS bit), N''Normal'', NULL),
    (2, ''2026-01-26T11:40:13.6886839Z'', NULL, N''Complimentary stay'', 2, CAST(1 AS bit), N''Complimentary'', NULL),
    (3, ''2026-01-26T11:40:13.6886841Z'', NULL, N''Family member stay'', 3, CAST(1 AS bit), N''Family'', NULL),
    (4, ''2026-01-26T11:40:13.6886843Z'', NULL, N''Hotel2 guest type'', 4, CAST(1 AS bit), N''Hotel2'', NULL),
    (5, ''2026-01-26T11:40:13.6886845Z'', NULL, N''Hotel3 guest type'', 5, CAST(1 AS bit), N''Hotel3'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'GuestTypeId', N'CreatedAt', N'DeletedAt', N'Description', N'DisplayOrder', N'IsActive', N'TypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[MGuestTypes]'))
        SET IDENTITY_INSERT [MGuestTypes] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    CREATE INDEX [IX_CheckIn_GuestTypeId] ON [CheckIn] ([GuestTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_MGuestTypes_GuestTypeId] FOREIGN KEY ([GuestTypeId]) REFERENCES [MGuestTypes] ([GuestTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126114016_AddGuestTypeFeature'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260126114016_AddGuestTypeFeature', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [IsPostedToVoucher] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD [PostedVoucherId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    EXEC(N'UPDATE [MGuestTypes] SET [CreatedAt] = ''2026-01-26T19:05:54.0799084Z''
    WHERE [GuestTypeId] = 1;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    EXEC(N'UPDATE [MGuestTypes] SET [CreatedAt] = ''2026-01-26T19:05:54.0799088Z''
    WHERE [GuestTypeId] = 2;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    EXEC(N'UPDATE [MGuestTypes] SET [CreatedAt] = ''2026-01-26T19:05:54.0799091Z''
    WHERE [GuestTypeId] = 3;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    EXEC(N'UPDATE [MGuestTypes] SET [CreatedAt] = ''2026-01-26T19:05:54.0799094Z''
    WHERE [GuestTypeId] = 4;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    EXEC(N'UPDATE [MGuestTypes] SET [CreatedAt] = ''2026-01-26T19:05:54.0799097Z''
    WHERE [GuestTypeId] = 5;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DataType', N'Description', N'IsSystemLocked', N'SettingKey', N'SettingValue', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[SystemSettings]'))
        SET IDENTITY_INSERT [SystemSettings] ON;
    EXEC(N'INSERT INTO [SystemSettings] ([Id], [CreatedAt], [CreatedBy], [DataType], [Description], [IsSystemLocked], [SettingKey], [SettingValue], [UpdatedAt], [UpdatedBy])
    VALUES (1, ''2026-01-26T19:05:54.0828154Z'', N''System'', N''Date'', N''Current business/working date for hotel operations'', CAST(1 AS bit), N''WorkingDate'', N''2026-01-27'', NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DataType', N'Description', N'IsSystemLocked', N'SettingKey', N'SettingValue', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[SystemSettings]'))
        SET IDENTITY_INSERT [SystemSettings] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_AdditionalCharges_PostedVoucherId] ON [AdditionalCharges] ([PostedVoucherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DayClosingAudit_ClosedDate] ON [DayClosingAudit] ([ClosedDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_SettingKey] ON [SystemSettings] ([SettingKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_Vouchers_AdditionalChargeId] ON [Vouchers] ([AdditionalChargeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_Vouchers_CheckInId] ON [Vouchers] ([CheckInId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_Vouchers_GuestId] ON [Vouchers] ([GuestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_Vouchers_VoucherDate] ON [Vouchers] ([VoucherDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vouchers_VoucherNumber] ON [Vouchers] ([VoucherNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    CREATE INDEX [IX_Vouchers_VoucherType] ON [Vouchers] ([VoucherType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    ALTER TABLE [AdditionalCharges] ADD CONSTRAINT [FK_AdditionalCharges_Vouchers_PostedVoucherId] FOREIGN KEY ([PostedVoucherId]) REFERENCES [Vouchers] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126190554_AddDayClosingAndVoucherSystem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260126190554_AddDayClosingAndVoucherSystem', N'8.0.0');
END;
GO

COMMIT;
GO

