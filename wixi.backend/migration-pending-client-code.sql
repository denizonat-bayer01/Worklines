-- Migration: AddPendingClientCode
-- This migration creates the PendingClientCode table for storing client codes before client profile creation

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'wixi_PendingClientCode')
BEGIN
    CREATE TABLE [wixi_PendingClientCode] (
        [Id] int NOT NULL IDENTITY,
        [ClientCode] nvarchar(50) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [ExpirationDate] datetime2 NOT NULL,
        [IsUsed] bit NOT NULL,
        [UsedAt] datetime2 NULL,
        [EmployeeSubmissionId] bigint NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_wixi_PendingClientCode] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_wixi_PendingClientCode_ClientCode] ON [wixi_PendingClientCode] ([ClientCode]);
    CREATE INDEX [IX_wixi_PendingClientCode_Email] ON [wixi_PendingClientCode] ([Email]);
    CREATE INDEX [IX_wixi_PendingClientCode_ExpirationDate] ON [wixi_PendingClientCode] ([ExpirationDate]);
    CREATE INDEX [IX_wixi_PendingClientCode_IsUsed] ON [wixi_PendingClientCode] ([IsUsed]);
END;
GO
