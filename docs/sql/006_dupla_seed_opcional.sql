SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- A posicao na chave deixa de ser obrigatoria no cadastro.
-- Ela passa a existir depois do sorteio. Varias duplas podem ficar sem seed.

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_duplas_torneio_id_seed' AND object_id = OBJECT_ID(N'dbo.duplas'))
    DROP INDEX [IX_duplas_torneio_id_seed] ON [dbo].[duplas];
GO

ALTER TABLE [dbo].[duplas] ALTER COLUMN [seed] int NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_duplas_torneio_id_seed' AND object_id = OBJECT_ID(N'dbo.duplas'))
    CREATE UNIQUE INDEX [IX_duplas_torneio_id_seed] ON [dbo].[duplas] ([torneio_id], [seed]) WHERE [is_deleted] = 0 AND [seed] IS NOT NULL;
GO
