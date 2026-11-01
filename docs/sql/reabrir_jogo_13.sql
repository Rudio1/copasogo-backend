SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Reabre o jogo 13 da edição em andamento, cancelado por engano.
-- Cancelar só troca o status: os slots e os jogos seguintes continuam intactos.
-- Volta para Upcoming; a gestão marca "ao vivo" de novo quando a partida começar.

-- 1) Conferir antes
SELECT t.nome, t.edicao, t.status AS torneio_status, p.numero, p.status, p.vencedor_dupla_id
FROM dbo.partidas AS p
INNER JOIN dbo.torneios AS t ON t.id = p.torneio_id
WHERE t.status = N'EmAndamento'
  AND t.is_deleted = 0
  AND p.is_deleted = 0
  AND p.numero = 13;

-- 2) Reabrir
BEGIN TRANSACTION;

UPDATE p
SET p.status = N'Upcoming',
    p.updated_at = SYSUTCDATETIME()
FROM dbo.partidas AS p
INNER JOIN dbo.torneios AS t ON t.id = p.torneio_id
WHERE t.status = N'EmAndamento'
  AND t.is_deleted = 0
  AND p.is_deleted = 0
  AND p.numero = 13
  AND p.status = N'Cancelled'
  AND p.vencedor_dupla_id IS NULL;

-- Só confirma se exatamente 1 linha mudou.
IF @@ROWCOUNT = 1
BEGIN
    COMMIT TRANSACTION;
    SELECT N'Jogo 13 reaberto' AS resultado;
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    SELECT N'Nada alterado: confira o SELECT acima' AS resultado;
END
GO
