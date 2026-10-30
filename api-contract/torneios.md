# Torneios

Uma edicao. Criar gera a chave double elimination: as rodadas fixas e os jogos 1–31, em `rascunho`. Nao cria as 16 duplas.

Slug omitido vira `copasogo-{edicao}`. Slug: letras minusculas, numeros e hifen. `slug` e `edicao` nao mudam depois.

So uma edicao `em_andamento`. Encerrar libera a proxima. Delete so em `rascunho` sem jogo `live` ou `completed` (apaga tambem duplas e partidas da edicao, soft delete).

## GET /api/torneios

Lista historica.

```json
{
  "id": "guid",
  "slug": "copasogo-1",
  "name": "COPASOGO",
  "edicao": 1,
  "status": "encerrado",
  "championId": "guid-ou-null",
  "championName": "Nome da dupla ou null"
}
```

## GET /api/torneios/atual

A edicao `em_andamento`. Se nenhuma estiver ao vivo, a ultima `encerrada`. 404 se nao houver nenhuma. Payload igual a `GET /api/torneios/{id}`.

A rota `atual` nao e um Guid: `{id}` so aceita Guid.

## GET /api/torneios/{id}

Payload que o site consome. `players` e o elenco da edicao (quem esta numa dupla), cada um com `steamId`. `paths` e um mapa `duplaId -> { past, next }` com numeros de jogo.

```json
{
  "id": "guid",
  "slug": "copasogo-1",
  "name": "COPASOGO",
  "edicao": 1,
  "status": "em_andamento",
  "entryFeeCents": 1000,
  "prizePoolCents": 32000,
  "iniciadoEm": null,
  "encerradoEm": null,
  "players": [{ "id": "guid", "name": "Nome", "steamId": "76561198000000000" }],
  "teams": [{ "id": "guid", "name": "Dupla", "seed": 1, "playerIds": ["guid", "guid"] }],
  "rounds": [
    { "id": "wb-r1", "side": "winners", "stage": "Rodada 1", "name": "Oitavas de final", "matchIds": [1, 2, 3, 4, 5, 6, 7, 8] }
  ],
  "matches": [
    {
      "id": 1,
      "torneioId": "guid",
      "side": "winners",
      "roundId": "wb-r1",
      "roundName": "Oitavas de final",
      "slotA": { "type": "team", "teamId": "guid", "matchId": null },
      "slotB": { "type": "team", "teamId": null, "matchId": null },
      "status": "pending",
      "scheduledAt": null,
      "score": null,
      "winnerTeamId": null
    }
  ],
  "standings": [{ "teamId": "guid", "state": "winners", "losses": 0 }],
  "champion": null,
  "paths": { "guid-da-dupla": { "past": [], "next": [1] } }
}
```

`champion` e o objeto de `teams` ou null. `roundName` e extra em relacao ao tipo `Match` do site. `torneioId` em cada jogo tambem.

Rodadas (sempre as mesmas):

| id | side | stage | name | jogos |
|---|---|---|---|---|
| wb-r1 | winners | Rodada 1 | Oitavas de final | 1–8 |
| wb-r2 | winners | Rodada 2 | Quartas de final | 9–12 |
| wb-r3 | winners | Rodada 3 | Semifinais | 13–14 |
| wb-r4 | winners | Rodada 4 | Final dos vencedores | 15 |
| lb-r1 | losers | | Rodada 1 | 16–19 |
| lb-r2 | losers | | Rodada 2 | 20–23 |
| lb-r3 | losers | | Rodada 3 | 24–25 |
| lb-r4 | losers | | Rodada 4 | 26–27 |
| lb-r5 | losers | | Rodada 5 | 28 |
| lb-r6 | losers | Rodada 6 | Final dos perdedores | 29 |
| gf | grand_final | | Grande final | 30 |
| gf-reset | grand_final | | Reset da final | 31 |

O status devolvido ja passou pela sincronizacao: o jogo 31 fica `cancelled` se o vencedor do 15 ganhar o 30; senao `upcoming` quando o 30 terminou pelo lado dos perdedores. Essa leitura nao grava sozinha; a proxima escrita persiste os status recalculados.

## POST /api/torneios

JWT. 201, `Location: /api/torneios/{id}`, body = payload completo (sem duplas).

```json
{
  "name": "COPASOGO",
  "slug": "copasogo-1",
  "edicao": 1,
  "entryFeeCents": 1000,
  "prizePoolCents": 32000
}
```

`slug` pode ser null. `edicao` >= 1, taxas >= 0, nome 1–120.

## PATCH /api/torneios/{id}

JWT. So nome e taxas, e so enquanto nao estiver `encerrado`.

```json
{ "name": "COPASOGO", "entryFeeCents": 1000, "prizePoolCents": 32000 }
```

## POST /api/torneios/{id}/iniciar

JWT. `rascunho` -> `em_andamento`. 422 se ja houver outra em andamento.

## POST /api/torneios/{id}/encerrar

JWT. `em_andamento` -> `encerrado`.

## DELETE /api/torneios/{id}

JWT. 204. So `rascunho` sem resultado.
