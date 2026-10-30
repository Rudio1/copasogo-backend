SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Edicoes. status no banco: Rascunho | EmAndamento | Encerrado
-- (a API traduz para rascunho | em_andamento | encerrado).

IF OBJECT_ID(N'dbo.torneios', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[torneios] (
        [id] uniqueidentifier NOT NULL,
        [nome] nvarchar(120) NOT NULL,
        [slug] nvarchar(80) NOT NULL,
        [edicao] int NOT NULL,
        [status] nvarchar(20) NOT NULL,
        [entry_fee_cents] int NOT NULL,
        [prize_pool_cents] int NOT NULL,
        [quantidade_partidas] int NOT NULL CONSTRAINT [DF_torneios_quantidade_partidas] DEFAULT 31,
        [iniciado_em] datetime2 NULL,
        [encerrado_em] datetime2 NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        [created_by] uniqueidentifier NULL,
        [updated_by] uniqueidentifier NULL,
        [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [deleted_at] datetime2 NULL,
        [deleted_by] uniqueidentifier NULL,
        CONSTRAINT [PK_torneios] PRIMARY KEY ([id])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_torneios_edicao' AND object_id = OBJECT_ID(N'dbo.torneios'))
    CREATE UNIQUE INDEX [IX_torneios_edicao] ON [dbo].[torneios] ([edicao]) WHERE [is_deleted] = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_torneios_slug' AND object_id = OBJECT_ID(N'dbo.torneios'))
    CREATE UNIQUE INDEX [IX_torneios_slug] ON [dbo].[torneios] ([slug]) WHERE [is_deleted] = 0;
GO
