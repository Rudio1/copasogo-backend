# Partidas

Os 31 jogos nascem com o torneio. Nao ha POST nem DELETE de partida, e nao existe jogo 32. O `{numero}` da rota e o id publico (1–31), nao o Guid interno.

Escrita so com a edicao editavel (`rascunho` ou `em_andamento`).

A resposta e o `Match` do site mais `torneioId` e `roundName`:

```json
{
  "id": 30,
  "torneioId": "guid",
  "side": "grand_final",
  "roundId": "gf",
  "roundName": "Grande final",
  "slotA": { "type": "winner_of", "teamId": null, "matchId": 15 },
  "slotB": { "type": "winner_of", "teamId": null, "matchId": 29 },
  "status": "upcoming",
  "scheduledAt": "2026-11-01T18:00:00.000Z",
  "score": { "a": 2, "b": 0 },
  "winnerTeamId": "guid"
}
```

`score` e null se os dois placares nao estiverem gravados.

## GET /api/torneios/{torneioId}/partidas

Publico. Filtros opcionais, combinaveis: `?roundId=wb-r1&status=upcoming&side=winners`.

## GET /api/torneios/{torneioId}/partidas/{numero}

Publico. 404 se o numero nao existir na edicao.

## PATCH /api/torneios/{torneioId}/partidas/{numero}

JWT. Agenda ou limpa o horario.

```json
{ "scheduledAt": "2026-11-01T18:00:00.000Z" }
```

`scheduledAt` null tira o agendamento.

## POST .../{numero}/ao-vivo

JWT. Sem body. So de `upcoming` (os dois slots ja resolvidos para uma dupla).

## POST .../{numero}/resultado

JWT. So em `upcoming` ou `live`. `winnerTeamId` tem de ser um dos participantes resolvidos.

```json
{
  "winnerTeamId": "guid",
  "score": { "a": 2, "b": 1 }
}
```

`score` e opcional, mas se vier precisa dos dois lados e o vencedor com placar estritamente maior. Depois do resultado a chave sincroniza: avanca vencedor/perdedor, abre o reset (31) ou o cancela se o campeao dos vencedores ganhou o 30. Duas derrotas eliminam a dupla.

W.O.: mande `"walkover": true`. O `score` e ignorado e a partida fica sem placar. Aceita `upcoming` ou `live` (a dupla ausente nao precisa ter entrado). A partida conta como derrota normal para quem nao compareceu. A resposta traz `walkover: true`. `POST .../placar` num jogo de W.O. lanca o placar e desfaz o W.O.

```json
{
  "winnerTeamId": "guid",
  "walkover": true
}
```

## POST .../{numero}/wo

JWT. Sem body. Transforma o resultado de um jogo `completed` em W.O.: o vencedor continua o mesmo, o placar e apagado e `walkover` vira `true`. A chave nao muda. Para desfazer, lance o placar em `POST .../placar`: isso volta `walkover` para `false`.

## POST .../{numero}/cancelar

JWT. Sem body. Nao cancela jogo `completed`.

## POST .../{numero}/reabrir

JWT. Sem body. Desfaz o cancelamento: so aceita jogo `cancelled`. O status volta a ser calculado pela chave (`upcoming` se as duas duplas estao definidas, senao `pending`). O reset (31) volta a `cancelled` se a grande final ja dispensou o jogo.

## Regras da chave

Constantes: final dos vencedores 15, final dos perdedores 29, grande final 30, reset 31.

Jogos 1–8 sao seeds. 9–15 recebem `winner_of` da chave de vencedores. 16–19 recebem `loser_of` de 1–8. 20–23 recebem o perdedor das quartas em ordem invertida: 20 ← 12, 21 ← 11, 22 ← 10, 23 ← 9. O restante segue o grafo double elimination do site (perdedor das quartas/semi/final dos vencedores desce). 30 e 31 tem os dois lados `winner_of` 15 e `winner_of` 29.
