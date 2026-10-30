SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Dupla pertence a uma edicao. seed unico por torneio entre linhas nao apagadas.
-- O mesmo jogador nao pode estar em duas duplas da mesma edicao: regra de dominio, nao de indice
-- (o jogador pode ser jogador_a ou jogador_b).

IF OBJECT_ID(N'dbo.duplas', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[duplas] (
        [id] uniqueidentifier NOT NULL,
        [torneio_id] uniqueidentifier NOT NULL,
        [nome] nvarchar(80) NOT NULL,
        [seed] int NULL,
        [jogador_a_id] uniqueidentifier NOT NULL,
        [jogador_b_id] uniqueidentifier NOT NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        [created_by] uniqueidentifier NULL,
        [updated_by] uniqueidentifier NULL,
        [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [deleted_at] datetime2 NULL,
        [deleted_by] uniqueidentifier NULL,
        CONSTRAINT [PK_duplas] PRIMARY KEY ([id]),
        CONSTRAINT [FK_duplas_jogadores_jogador_a_id] FOREIGN KEY ([jogador_a_id]) REFERENCES [dbo].[jogadores] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_duplas_jogadores_jogador_b_id] FOREIGN KEY ([jogador_b_id]) REFERENCES [dbo].[jogadores] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_duplas_torneios_torneio_id] FOREIGN KEY ([torneio_id]) REFERENCES [dbo].[torneios] ([id]) ON DELETE NO ACTION
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_duplas_jogador_a_id' AND object_id = OBJECT_ID(N'dbo.duplas'))
    CREATE INDEX [IX_duplas_jogador_a_id] ON [dbo].[duplas] ([jogador_a_id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_duplas_jogador_b_id' AND object_id = OBJECT_ID(N'dbo.duplas'))
    CREATE INDEX [IX_duplas_jogador_b_id] ON [dbo].[duplas] ([jogador_b_id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_duplas_torneio_id_seed' AND object_id = OBJECT_ID(N'dbo.duplas'))
    CREATE UNIQUE INDEX [IX_duplas_torneio_id_seed] ON [dbo].[duplas] ([torneio_id], [seed]) WHERE [is_deleted] = 0 AND [seed] IS NOT NULL;
GO
