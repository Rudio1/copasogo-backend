# Enums

Na API, sempre string. No banco, o nome do enum C# (ver `docs/sql/README.md`). Nao misture os dois.

## status do torneio

| API | Significado |
|---|---|
| `rascunho` | Criada, ainda nao iniciada. Unica que pode ser apagada |
| `em_andamento` | No maximo uma edicao neste estado |
| `encerrado` | Historico. Nao volta atras e nao se apaga |

## status da partida

`pending` | `upcoming` | `live` | `completed` | `cancelled`

- `pending`: ainda falta slot resolvido
- `upcoming`: os dois lados ja tem dupla
- `live`: em andamento (so a partir de `upcoming`)
- `completed`: tem vencedor
- `cancelled`: cancelada a mao, ou o jogo 31 quando o campeao da chave de vencedores ganha o jogo 30

`completed`, `live` e `cancelled` nao sao reescritos pela sincronizacao automatica.

## side

`winners` | `losers` | `grand_final`

## slot.type

`team` | `winner_of` | `loser_of`

- `team`: `teamId` (Guid ou null enquanto a seed nao tem dupla). `matchId` null
- `winner_of` / `loser_of`: `matchId` e o numero do jogo de origem. `teamId` null

## standing.state

`winners` | `losers` | `eliminated` | `champion` | `finalist`

`losses` e 0, 1 ou 2. Duas derrotas eliminam.
