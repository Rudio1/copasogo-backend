# Duplas

Sempre de uma edicao. No maximo 16, seeds 1–16, exatamente dois jogadores distintos. O mesmo jogador nao entra em duas duplas da mesma edicao (pode em outra).

Criar a dupla liga o `teamId` no slot `team` da oitava correspondente a seed (seed 1 no jogo 1 slot A, seed 2 no jogo 1 slot B, e assim por diante).

Edicao `encerrada` nao aceita escrita. Mudar seed ou elenco e proibido se a dupla ja entrou em jogo `live` ou `completed` daquela edicao. Renomear ainda e permitido. Delete tem a mesma trava.

## GET /api/torneios/{torneioId}/duplas

Publico. Lista de `Dupla`.

## GET /api/duplas/{id}

Publico.

```json
{
  "id": "guid",
  "torneioId": "guid",
  "name": "Dupla",
  "seed": 1,
  "playerIds": ["guid", "guid"],
  "players": [
    { "id": "guid", "name": "A", "steamId": "76561198000000000" },
    { "id": "guid", "name": "B", "steamId": "76561198000000001" }
  ],
  "standing": { "teamId": "guid", "state": "winners", "losses": 0 },
  "path": { "past": [], "next": [1] }
}
```

## POST /api/torneios/{torneioId}/duplas

JWT. 201, `Location: /api/duplas/{id}`.

```json
{
  "name": "Dupla",
  "seed": 1,
  "playerIds": ["guid-jogador-a", "guid-jogador-b"]
}
```

`playerIds` precisa ter comprimento 2. 422 se a seed ja existir, se um jogador ja estiver noutra dupla da edicao, ou se passar de 16.

## PUT /api/duplas/{id}

JWT. Mesmo body. O torneio e o da dupla; nao muda de edicao.

## DELETE /api/duplas/{id}

JWT. 204. Solta o slot da seed.
