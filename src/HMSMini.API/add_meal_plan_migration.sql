BEGIN TRANSACTION;
GO

ALTER TABLE [Reservations] ADD [MealPlanId] int NULL;
GO

ALTER TABLE [CheckIn] ADD [MealPlanId] int NULL;
GO

ALTER TABLE [CheckIn] ADD [MealPlanRate] decimal(10,2) NULL;
GO

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
GO

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
GO

CREATE INDEX [IX_Reservations_MealPlanId] ON [Reservations] ([MealPlanId]);
GO

CREATE INDEX [IX_CheckIn_MealPlanId] ON [CheckIn] ([MealPlanId]);
GO

CREATE INDEX [IX_MealPlanRates_DeletedAt] ON [MealPlanRates] ([DeletedAt]);
GO

CREATE INDEX [IX_MealPlanRates_IsActive] ON [MealPlanRates] ([IsActive]);
GO

CREATE INDEX [IX_MealPlanRates_MealPlan_RoomType_EffectiveFrom] ON [MealPlanRates] ([MealPlanId], [RoomTypeId], [EffectiveFrom]);
GO

CREATE INDEX [IX_MealPlanRates_RoomTypeId] ON [MealPlanRates] ([RoomTypeId]);
GO

CREATE INDEX [IX_MMealPlans_DeletedAt] ON [MMealPlans] ([DeletedAt]);
GO

CREATE INDEX [IX_MMealPlans_IsActive] ON [MMealPlans] ([IsActive]);
GO

CREATE UNIQUE INDEX [IX_MMealPlans_PlanCode] ON [MMealPlans] ([PlanCode]);
GO

ALTER TABLE [CheckIn] ADD CONSTRAINT [FK_CheckIn_MMealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MMealPlans] ([MealPlanId]) ON DELETE NO ACTION;
GO

ALTER TABLE [Reservations] ADD CONSTRAINT [FK_Reservations_MMealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MMealPlans] ([MealPlanId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260121122456_AddMealPlanTables', N'8.0.0');
GO

COMMIT;
GO

