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

## POST .../{numero}/cancelar

JWT. Sem body. Nao cancela jogo `completed`.

## Regras da chave

Constantes: final dos vencedores 15, final dos perdedores 29, grande final 30, reset 31.

Jogos 1–8 sao seeds. 9–15 recebem `winner_of` da chave de vencedores. 16–19 recebem `loser_of` de 1–8. O restante segue o grafo double elimination do site (perdedor das quartas/semi/final dos vencedores desce). 30 e 31 tem os dois lados `winner_of` 15 e `winner_of` 29.
