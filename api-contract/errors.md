# Erros

| HTTP | code | Quando |
|---|---|---|
| 400 | `VALIDATION_ERROR` | Body invalido (FluentValidation) ou dupla sem exatamente dois `playerIds`. `error.fields` lista `{ field, message }` |
| 401 | `UNAUTHORIZED` | Login recusado, ou escrita sem token / token invalido (este ultimo pode vir sem o envelope, pelo JWT bearer) |
| 403 | `UNAUTHORIZED` | Token valido sem role `Administrador` |
| 404 | `NOT_FOUND` | Id, numero de jogo ou edicao atual inexistente |
| 422 | `BUSINESS_ERROR` | Regra de dominio (edicao encerrada, seed repetida, Steam ID duplicado, resultado ilegal, ja existe edicao em andamento) |
| 500 | `INTERNAL_ERROR` | Falha nao mapeada. A mensagem publica nao inclui detalhe interno |

Mensagens de dominio sao texto simples, sem acento (ex.: `Torneio nao encontrado.`).

Apagar edicao encerrada, apagar jogador que ja entrou em jogo `live` ou `completed`, e alterar elenco de dupla que ja jogou naquela edicao, caem em 422.
