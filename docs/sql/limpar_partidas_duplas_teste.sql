SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Limpa a chave e as duplas de teste da COPASOGO, edição 1.
-- A edição e os jogadores permanecem.
-- Partidas saem antes: slot e vencedor referenciam dupla, sem cascade.

BEGIN TRANSACTION;

DELETE FROM dbo.partidas
WHERE torneio_id IN (
    SELECT id
    FROM dbo.torneios
    WHERE nome = N'COPASOGO'
      AND edicao = 1
      AND is_deleted = 0
);

DELETE FROM dbo.duplas
WHERE torneio_id IN (
    SELECT id
    FROM dbo.torneios
    WHERE nome = N'COPASOGO'
      AND edicao = 1
      AND is_deleted = 0
);

COMMIT TRANSACTION;
GO
