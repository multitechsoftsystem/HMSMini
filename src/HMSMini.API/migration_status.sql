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

