SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 31 jogos por edicao (numero 1-31). Nao ha jogo 32.
-- Colunas de enum guardam o nome CLR, nao a string da API:
--   lado:   Winners | Losers | GrandFinal
--   status: Pending | Upcoming | Live | Completed | Cancelled
--   slot_*_tipo: Team | WinnerOf | LoserOf
-- Rodadas nao tem tabela: o grafo fica em ChaveFabrica e rodada_id aponta para ele (wb-r1, lb-r1, gf, gf-reset).

IF OBJECT_ID(N'dbo.partidas', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[partidas] (
        [id] uniqueidentifier NOT NULL,
        [torneio_id] uniqueidentifier NOT NULL,
        [numero] int NOT NULL,
        [lado] nvarchar(20) NOT NULL,
        [rodada_id] nvarchar(20) NOT NULL,
        [slot_a_tipo] nvarchar(20) NOT NULL,
        [slot_a_dupla_id] uniqueidentifier NULL,
        [slot_a_seed] int NULL,
        [slot_a_origem] int NULL,
        [slot_b_tipo] nvarchar(20) NOT NULL,
        [slot_b_dupla_id] uniqueidentifier NULL,
        [slot_b_seed] int NULL,
        [slot_b_origem] int NULL,
        [status] nvarchar(20) NOT NULL,
        [agendada_em] datetime2 NULL,
        [placar_a] int NULL,
        [placar_b] int NULL,
        [vencedor_dupla_id] uniqueidentifier NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        [created_by] uniqueidentifier NULL,
        [updated_by] uniqueidentifier NULL,
        [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [deleted_at] datetime2 NULL,
        [deleted_by] uniqueidentifier NULL,
        CONSTRAINT [PK_partidas] PRIMARY KEY ([id]),
        CONSTRAINT [FK_partidas_duplas_slot_a_dupla_id] FOREIGN KEY ([slot_a_dupla_id]) REFERENCES [dbo].[duplas] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_partidas_duplas_slot_b_dupla_id] FOREIGN KEY ([slot_b_dupla_id]) REFERENCES [dbo].[duplas] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_partidas_duplas_vencedor_dupla_id] FOREIGN KEY ([vencedor_dupla_id]) REFERENCES [dbo].[duplas] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_partidas_torneios_torneio_id] FOREIGN KEY ([torneio_id]) REFERENCES [dbo].[torneios] ([id]) ON DELETE NO ACTION
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_partidas_slot_a_dupla_id' AND object_id = OBJECT_ID(N'dbo.partidas'))
    CREATE INDEX [IX_partidas_slot_a_dupla_id] ON [dbo].[partidas] ([slot_a_dupla_id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_partidas_slot_b_dupla_id' AND object_id = OBJECT_ID(N'dbo.partidas'))
    CREATE INDEX [IX_partidas_slot_b_dupla_id] ON [dbo].[partidas] ([slot_b_dupla_id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_partidas_torneio_id_numero' AND object_id = OBJECT_ID(N'dbo.partidas'))
    CREATE UNIQUE INDEX [IX_partidas_torneio_id_numero] ON [dbo].[partidas] ([torneio_id], [numero]) WHERE [is_deleted] = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_partidas_vencedor_dupla_id' AND object_id = OBJECT_ID(N'dbo.partidas'))
    CREATE INDEX [IX_partidas_vencedor_dupla_id] ON [dbo].[partidas] ([vencedor_dupla_id]);
GO
