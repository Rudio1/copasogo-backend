# Jogadores

Catalogo global. A mesma pessoa (mesmo Steam ID) pode jogar varias edicoes, cada uma numa dupla diferente. Nao ha teto global.

`steamId` e SteamID64: 17 digitos comecando em `7656119`. Unico entre jogadores nao apagados. Nesta fatia a API nao chama a Steam; so guarda o id.

## GET /api/jogadores

Publico. `?torneioId={guid}` devolve so quem esta numa dupla daquela edicao. Sem o query, o catalogo inteiro.

```json
{
  "id": "guid",
  "name": "Nome",
  "steamId": "76561198000000000",
  "teamId": null,
  "teamName": null,
  "partner": null,
  "standing": null
}
```

Com `torneioId`, `teamId`, `teamName`, `partner` (`{ id, name, steamId }`) e `standing` (`winners`, `losers`, `eliminated`, `champion`, `finalist`) vem preenchidos.

## GET /api/jogadores/{id}

Publico. Mesmo objeto. `?torneioId=` acrescenta dupla e parceiro daquela edicao.

## POST /api/jogadores

JWT. 201.

```json
{ "name": "Nome", "steamId": "76561198000000000" }
```

Nome 1–80.

## PUT /api/jogadores/{id}

JWT. Mesmo body. Steam ID nao pode colidir com outro jogador.

## DELETE /api/jogadores/{id}

JWT. 204. 422 se o jogador ja entrou em partida `live` ou `completed` de qualquer edicao.
