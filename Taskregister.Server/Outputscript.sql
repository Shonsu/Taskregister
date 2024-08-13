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

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tasks] (
    [Id] int NOT NULL IDENTITY,
    [Type] int NOT NULL,
    [Priority] int NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [State] int NOT NULL,
    [DateState] datetime2 NOT NULL,
    [Description] nvarchar(max) NULL,
    [ChangeStateRationale] nvarchar(max) NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tasks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_Tasks_UserId] ON [Tasks] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240622202219_initial', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Users_UserId];
GO

DROP INDEX [IX_Tasks_UserId] ON [Tasks];
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'UserId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var0 + '];');
UPDATE [Tasks] SET [UserId] = 0 WHERE [UserId] IS NULL;
ALTER TABLE [Tasks] ALTER COLUMN [UserId] int NOT NULL;
ALTER TABLE [Tasks] ADD DEFAULT 0 FOR [UserId];
CREATE INDEX [IX_Tasks_UserId] ON [Tasks] ([UserId]);
GO

ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240623175044_AddUserIdToTaskEntity', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Tasks].[ChangeStateRationale]', N'ChangeEndDateRationale', N'COLUMN';
GO

ALTER TABLE [Tasks] ADD [CreateAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240624164212_AddCreateAtAndRenameColumn', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Tasks] ADD [History] nvarchar(max) NOT NULL DEFAULT N'[]';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240628172032_AddTaskHisotry', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'State');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Tasks] ALTER COLUMN [State] nvarchar(max) NOT NULL;
GO

UPDATE Tasks SET State = 'New' WHERE State = '0'
GO

UPDATE Tasks SET State = 'Completed' WHERE State = '1'
GO

UPDATE Tasks SET State = 'Resumed' WHERE State = '2'
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240804091145_TaskEntityAddConversionToStateEnum', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Users_UserId];
GO

ALTER TABLE [Tasks] DROP CONSTRAINT [PK_Tasks];
GO

EXEC sp_rename N'[Tasks]', N'Todos';
GO

EXEC sp_rename N'[Todos].[IX_Tasks_UserId]', N'IX_Todos_UserId', N'INDEX';
GO

ALTER TABLE [Todos] ADD CONSTRAINT [PK_Todos] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Todos] ADD CONSTRAINT [FK_Todos_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240807104959_ChangeTaskNameToTodo1', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TagTodo] (
    [TagsId] int NOT NULL,
    [TodosId] int NOT NULL,
    CONSTRAINT [PK_TagTodo] PRIMARY KEY ([TagsId], [TodosId]),
    CONSTRAINT [FK_TagTodo_Tags_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TagTodo_Todos_TodosId] FOREIGN KEY ([TodosId]) REFERENCES [Todos] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_TagTodo_TodosId] ON [TagTodo] ([TodosId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240807140139_addTagEntityAndRelationRoTodo', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tags]') AND [c].[name] = N'Value');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Tags] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Tags] ALTER COLUMN [Value] nvarchar(450) NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Tags_Value] ON [Tags] ([Value]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240807194347_AddIndexUniqueConstraintOnValueInTags', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Tags].[Value]', N'Name', N'COLUMN';
GO

EXEC sp_rename N'[Tags].[IX_Tags_Value]', N'IX_Tags_Name', N'INDEX';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240808084601_TagsRenameColumnName', N'8.0.6');
GO

COMMIT;
GO

