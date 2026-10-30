# Contrato da API Copasogo

Fonte da verdade HTTP para o site. O frontend Next ainda le `data/*.ts`; esta fatia so publica o contrato.

**Regra:** ao criar, alterar ou remover rota, DTO, enum ou formato de erro, atualize os arquivos aqui no mesmo PR.

Base local: `http://localhost:5288` (https `https://localhost:7288`). Swagger em Development: `/swagger`.

## Envelope

Sucesso:

```json
{ "success": true, "data": {}, "error": null }
```

Erro:

```json
{
  "success": false,
  "data": null,
  "error": { "code": "BUSINESS_ERROR", "message": "...", "fields": null }
}
```

JSON em camelCase. Datas em UTC (`yyyy-MM-ddTHH:mm:ss.fffZ`). Enums da API sao string minuscula (ver `enums.md`). Ids de torneio, jogador e dupla sao Guid em string. O id publico da partida e o numero 1–31.

## Auth

Escrita exige `Authorization: Bearer {accessToken}` e a role `Administrador`. GETs abaixo sao anonimos. Nao ha register, refresh nem cookie.

## Rotas

| Metodo | Rota | Auth | Doc |
|---|---|---|---|
| GET | `/health` | Publico | Saude do host |
| GET | `/api/health` | Publico | Saude da API |
| POST | `/api/auth/login` | Publico | [auth.md](auth.md) |
| GET | `/api/torneios` | Publico | [torneios.md](torneios.md) |
| GET | `/api/torneios/atual` | Publico | [torneios.md](torneios.md) |
| GET | `/api/torneios/{id}` | Publico | [torneios.md](torneios.md) |
| POST | `/api/torneios` | JWT | [torneios.md](torneios.md) |
| PATCH | `/api/torneios/{id}` | JWT | [torneios.md](torneios.md) |
| POST | `/api/torneios/{id}/iniciar` | JWT | [torneios.md](torneios.md) |
| POST | `/api/torneios/{id}/encerrar` | JWT | [torneios.md](torneios.md) |
| DELETE | `/api/torneios/{id}` | JWT | [torneios.md](torneios.md) |
| GET | `/api/jogadores` | Publico | [jogadores.md](jogadores.md) |
| GET | `/api/jogadores/{id}` | Publico | [jogadores.md](jogadores.md) |
| POST | `/api/jogadores` | JWT | [jogadores.md](jogadores.md) |
| PUT | `/api/jogadores/{id}` | JWT | [jogadores.md](jogadores.md) |
| DELETE | `/api/jogadores/{id}` | JWT | [jogadores.md](jogadores.md) |
| GET | `/api/torneios/{torneioId}/duplas` | Publico | [duplas.md](duplas.md) |
| GET | `/api/duplas/{id}` | Publico | [duplas.md](duplas.md) |
| POST | `/api/torneios/{torneioId}/duplas` | JWT | [duplas.md](duplas.md) |
| PUT | `/api/duplas/{id}` | JWT | [duplas.md](duplas.md) |
| DELETE | `/api/duplas/{id}` | JWT | [duplas.md](duplas.md) |
| GET | `/api/torneios/{torneioId}/partidas` | Publico | [partidas.md](partidas.md) |
| GET | `/api/torneios/{torneioId}/partidas/{numero}` | Publico | [partidas.md](partidas.md) |
| PATCH | `/api/torneios/{torneioId}/partidas/{numero}` | JWT | [partidas.md](partidas.md) |
| POST | `/api/torneios/{torneioId}/partidas/{numero}/ao-vivo` | JWT | [partidas.md](partidas.md) |
| POST | `/api/torneios/{torneioId}/partidas/{numero}/resultado` | JWT | [partidas.md](partidas.md) |
| POST | `/api/torneios/{torneioId}/partidas/{numero}/cancelar` | JWT | [partidas.md](partidas.md) |

O `id: "copasogo"` fixo do site vira Guid. Na integracao, a edicao vigente e `GET /api/torneios/atual`; o slug (`copasogo-1`) e `edicao` identificam o arquivo historico.

Fora desta fatia: Twitch, fotos, nick extra, VOD, divisao de premio.

Ver tambem: [enums.md](enums.md), [errors.md](errors.md).
