CREATE TABLE [MRoomTypes] (
    [RoomTypeId] int NOT NULL IDENTITY,
    [RoomType] nvarchar(50) NOT NULL,
    [RoomDescription] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_MRoomTypes] PRIMARY KEY ([RoomTypeId])
);
GO


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
GO


CREATE TABLE [RoomNo] (
    [RoomId] int NOT NULL IDENTITY,
    [RoomNumber] nvarchar(20) NOT NULL,
    [RoomTypeId] int NOT NULL,
    [RoomStatus] int NOT NULL,
    [RoomStatusFromDate] datetime2 NULL,
    [RoomStatusToDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_RoomNo] PRIMARY KEY ([RoomId]),
    CONSTRAINT [FK_RoomNo_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [CheckIn] (
    [Id] int NOT NULL IDENTITY,
    [RoomId] int NOT NULL,
    [CheckInDate] datetime2 NOT NULL,
    [CheckOutDate] datetime2 NOT NULL,
    [ActualCheckInDate] datetime2 NULL,
    [ActualCheckOutDate] datetime2 NULL,
    [RegistrationNo] nvarchar(50) NULL,
    [Pax] int NOT NULL,
    [Status] int NOT NULL,
    [Remarks] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_CheckIn] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CheckIn_RoomNo_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [RoomNo] ([RoomId]) ON DELETE NO ACTION
);
GO


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
    [PanOrAadharNo] nvarchar(20) NULL,
    [Photo1Path] nvarchar(500) NULL,
    [Photo2Path] nvarchar(500) NULL,
    [ActualCheckInDate] datetime2 NULL,
    [ActualCheckOutDate] datetime2 NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Guest] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Guest_CheckIn_CheckInId] FOREIGN KEY ([CheckInId]) REFERENCES [CheckIn] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Reservations] (
    [Id] int NOT NULL IDENTITY,
    [ReservationNumber] nvarchar(50) NOT NULL,
    [RoomTypeId] int NOT NULL,
    [RoomId] int NULL,
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
    CONSTRAINT [FK_Reservations_MRoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [MRoomTypes] ([RoomTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reservations_RoomNo_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [RoomNo] ([RoomId]) ON DELETE NO ACTION
);
GO


CREATE INDEX [IX_CheckIn_CheckInDate] ON [CheckIn] ([CheckInDate]);
GO


CREATE INDEX [IX_CheckIn_CheckOutDate] ON [CheckIn] ([CheckOutDate]);
GO


CREATE INDEX [IX_CheckIn_RoomId_Status] ON [CheckIn] ([RoomId], [Status]);
GO


CREATE INDEX [IX_CheckIn_Status] ON [CheckIn] ([Status]);
GO


CREATE UNIQUE INDEX [IX_Guest_CheckInId_GuestNumber] ON [Guest] ([CheckInId], [GuestNumber]);
GO


CREATE UNIQUE INDEX [IX_MRoomTypes_RoomType] ON [MRoomTypes] ([RoomType]);
GO


CREATE INDEX [IX_Reservations_CheckInDate] ON [Reservations] ([CheckInDate]);
GO


CREATE INDEX [IX_Reservations_CheckInId] ON [Reservations] ([CheckInId]);
GO


CREATE UNIQUE INDEX [IX_Reservations_ReservationNumber] ON [Reservations] ([ReservationNumber]);
GO


CREATE INDEX [IX_Reservations_RoomId_CheckInDate_CheckOutDate] ON [Reservations] ([RoomId], [CheckInDate], [CheckOutDate]);
GO


CREATE INDEX [IX_Reservations_RoomTypeId] ON [Reservations] ([RoomTypeId]);
GO


CREATE INDEX [IX_Reservations_Status] ON [Reservations] ([Status]);
GO


CREATE UNIQUE INDEX [IX_RoomNo_RoomNumber] ON [RoomNo] ([RoomNumber]);
GO


CREATE INDEX [IX_RoomNo_RoomStatus] ON [RoomNo] ([RoomStatus]);
GO


CREATE INDEX [IX_RoomNo_RoomStatus_RoomStatusFromDate_RoomStatusToDate] ON [RoomNo] ([RoomStatus], [RoomStatusFromDate], [RoomStatusToDate]);
GO


CREATE INDEX [IX_RoomNo_RoomTypeId] ON [RoomNo] ([RoomTypeId]);
GO


CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO


CREATE INDEX [IX_Users_IsActive] ON [Users] ([IsActive]);
GO


CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO


