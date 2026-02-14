-- Migration script to add Guest Type feature
-- Run this script on HMSMiniDB database

USE HMSMiniDB;
GO

-- Create MGuestTypes table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MGuestTypes')
BEGIN
    CREATE TABLE [dbo].[MGuestTypes] (
        [GuestTypeId] INT NOT NULL IDENTITY(1,1),
        [TypeName] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [DisplayOrder] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [DeletedAt] DATETIME2 NULL,
        CONSTRAINT [PK_MGuestTypes] PRIMARY KEY ([GuestTypeId])
    );

    PRINT 'MGuestTypes table created successfully.';

    -- Insert seed data
    SET IDENTITY_INSERT [dbo].[MGuestTypes] ON;

    INSERT INTO [dbo].[MGuestTypes] ([GuestTypeId], [TypeName], [Description], [IsActive], [DisplayOrder], [CreatedAt])
    VALUES
        (1, 'Normal', 'Regular paying guests', 1, 1, GETUTCDATE()),
        (2, 'Complimentary', 'Complimentary/Free stay guests', 1, 2, GETUTCDATE()),
        (3, 'Family', 'Family members or special relations', 1, 3, GETUTCDATE()),
        (4, 'Hotel2', 'Guests from partner Hotel 2', 1, 4, GETUTCDATE()),
        (5, 'Hotel3', 'Guests from partner Hotel 3', 1, 5, GETUTCDATE());

    SET IDENTITY_INSERT [dbo].[MGuestTypes] OFF;

    PRINT 'Seed data inserted into MGuestTypes table.';
END
ELSE
BEGIN
    PRINT 'MGuestTypes table already exists.';
END
GO

-- Add GuestTypeId column to CheckIns table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CheckIns') AND name = 'GuestTypeId')
BEGIN
    ALTER TABLE [dbo].[CheckIns]
    ADD [GuestTypeId] INT NULL;

    PRINT 'GuestTypeId column added to CheckIns table.';

    -- Add foreign key constraint
    ALTER TABLE [dbo].[CheckIns]
    ADD CONSTRAINT [FK_CheckIns_MGuestTypes_GuestTypeId]
    FOREIGN KEY ([GuestTypeId]) REFERENCES [dbo].[MGuestTypes]([GuestTypeId]);

    PRINT 'Foreign key constraint added.';
END
ELSE
BEGIN
    PRINT 'GuestTypeId column already exists in CheckIns table.';
END
GO

PRINT 'Guest Type migration completed successfully!';
GO
