SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Rodada 2 da chave dos perdedores passa a receber as quartas invertidas:
--   jogo 20 <- perdedor do 12, 21 <- 11, 22 <- 10, 23 <- 9.
-- ChaveFabrica já gera assim nas edições novas. Este script corrige as partidas
-- que já existem em edições de 31 jogos.
-- O slot só guarda a origem; quem joga é resolvido na leitura. Por isso basta trocar slot_b_origem,
-- mesmo que as quartas já tenham resultado. Jogos 20-23 ao vivo ou concluídos não são tocados.
-- Idempotente: só altera linhas que ainda estão com a origem antiga.

BEGIN TRANSACTION;

UPDATE p
SET p.slot_b_origem = 21 - p.slot_b_origem, -- 9<->12, 10<->11
    p.updated_at = SYSUTCDATETIME()
FROM dbo.partidas AS p
INNER JOIN dbo.torneios AS t ON t.id = p.torneio_id
WHERE t.quantidade_partidas = 31
  AND p.is_deleted = 0
  AND p.slot_b_tipo = N'LoserOf'
  AND p.status NOT IN (N'Live', N'Completed')
  AND (
       (p.numero = 20 AND p.slot_b_origem = 9)
    OR (p.numero = 21 AND p.slot_b_origem = 10)
    OR (p.numero = 22 AND p.slot_b_origem = 11)
    OR (p.numero = 23 AND p.slot_b_origem = 12)
  );

SELECT t.nome, t.edicao, p.numero, p.slot_b_origem, p.status
FROM dbo.partidas AS p
INNER JOIN dbo.torneios AS t ON t.id = p.torneio_id
WHERE p.numero BETWEEN 20 AND 23
  AND p.is_deleted = 0
ORDER BY t.edicao, p.numero;

COMMIT TRANSACTION;
GO
