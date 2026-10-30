SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_jogadores_steam_id' AND object_id = OBJECT_ID(N'dbo.jogadores'))
    DROP INDEX [IX_jogadores_steam_id] ON [dbo].[jogadores];
GO

ALTER TABLE [dbo].[jogadores] ALTER COLUMN [steam_id] nvarchar(200) NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_jogadores_steam_id' AND object_id = OBJECT_ID(N'dbo.jogadores'))
    CREATE UNIQUE INDEX [IX_jogadores_steam_id] ON [dbo].[jogadores] ([steam_id]) WHERE [is_deleted] = 0;
GO
