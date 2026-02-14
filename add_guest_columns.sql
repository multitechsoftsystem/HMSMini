-- Add columns to Guest table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Guest]') AND name = 'ActualCheckInDate')
BEGIN
    ALTER TABLE [Guest] ADD [ActualCheckInDate] datetime2 NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Guest]') AND name = 'ActualCheckOutDate')
BEGIN
    ALTER TABLE [Guest] ADD [ActualCheckOutDate] datetime2 NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Guest]') AND name = 'Status')
BEGIN
    ALTER TABLE [Guest] ADD [Status] int NOT NULL DEFAULT 0;
END

PRINT 'Guest table columns added successfully';
