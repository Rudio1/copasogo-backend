SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Catalogo global. steam_id e o SteamID64 ou o link do perfil.

IF OBJECT_ID(N'dbo.jogadores', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[jogadores] (
        [id] uniqueidentifier NOT NULL,
        [nome] nvarchar(80) NOT NULL,
        [steam_id] nvarchar(200) NOT NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        [created_by] uniqueidentifier NULL,
        [updated_by] uniqueidentifier NULL,
        [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [deleted_at] datetime2 NULL,
        [deleted_by] uniqueidentifier NULL,
        CONSTRAINT [PK_jogadores] PRIMARY KEY ([id])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_jogadores_steam_id' AND object_id = OBJECT_ID(N'dbo.jogadores'))
    CREATE UNIQUE INDEX [IX_jogadores_steam_id] ON [dbo].[jogadores] ([steam_id]) WHERE [is_deleted] = 0;
GO
