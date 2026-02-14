BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reservations]') AND [c].[name] = N'EstimatedAmount');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Reservations] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Reservations] ALTER COLUMN [EstimatedAmount] decimal(10,2) NULL;
GO

ALTER TABLE [Reservations] ADD [BusinessSourceId] int NULL;
GO

ALTER TABLE [CheckIn] ADD [BusinessSourceId] int NULL;
GO

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
GO

CREATE INDEX [IX_Reservations_BusinessSourceId] ON [Reservations] ([BusinessSourceId]);
GO

CREATE INDEX [IX_CheckIn_BusinessSourceId] ON [CheckIn] ([BusinessSourceId]);
GO

CREATE INDEX [IX_MBusinessSources_DeletedAt] ON [MBusinessSources] ([DeletedAt]);
GO

CREATE INDEX [IX_MBusinessSources_IsActive] ON [MBusinessSources] ([IsActive]);
GO

CREATE UNIQUE INDEX [IX_MBusinessSources_SourceName] ON [MBusinessSources] ([SourceName]);
GO

ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_MBusinessSources_BusinessSourceId] FOREIGN KEY ([BusinessSourceId]) REFERENCES [MBusinessSources] ([BusinessSourceId]) ON DELETE NO ACTION;
GO

ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MBusinessSources_BusinessSourceId] FOREIGN KEY ([BusinessSourceId]) REFERENCES [MBusinessSources] ([BusinessSourceId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260121103024_AddBusinessSourceTables', N'8.0.0');
GO

COMMIT;
GO

