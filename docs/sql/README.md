# Scripts SQL

O schema nao e criado por migration. Rode estes arquivos, nesta ordem, no banco `copasogo`.

A connection string de desenvolvimento fica em `appsettings.Development.json` (gitignored). No `sqlcmd`, use `-I` para ligar `QUOTED_IDENTIFIER`: os indices filtrados (`WHERE is_deleted = 0`) falham sem isso.

Crie o banco antes, se ainda nao existir:

```sql
IF DB_ID(N'copasogo') IS NULL
    CREATE DATABASE [copasogo];
```

## Ordem

1. `001_identity.sql` — tabelas ASP.NET Identity (`AspNet*`, usuario Guid, coluna `Nome`)
2. `002_torneios.sql` — edicoes
3. `003_jogadores.sql` — catalogo global (`steam_id` unico)
4. `004_duplas.sql` — duplas por edicao
5. `005_partidas.sql` — jogos 1–31 por edicao
6. `006_dupla_seed_opcional.sql` — seed da dupla passa a ser opcional ate o sorteio
7. `007_torneio_quantidade_partidas.sql` — tamanho da chave da edicao
8. `008_jogador_steam_id.sql` — steam_id aceita link ou identificador personalizado
9. `009_perdedores_rodada2_invertida.sql` — rodada 2 dos perdedores recebe as quartas invertidas (corrige edicoes ja criadas)
10. `010_partida_wo.sql` — coluna `wo` para vitoria por W.O.

Cada script e idempotente (`IF OBJECT_ID` / `IF NOT EXISTS`). Pode rodar de novo sem dropar o que ja existe.

Nao ha seed de elenco nem de edicao. A primeira edicao nasce em `POST /api/torneios`.

## Admin

Nao ha `INSERT` de usuario nestes scripts. Depois do SQL, configure (user secrets ou `appsettings.Development.json`, que esta no `.gitignore`):

```json
"Jwt": {
  "SigningKey": "uma-chave-com-pelo-menos-32-caracteres"
},
"Admin": {
  "Email": "admin@copasogo.local",
  "Senha": "TroqueEsta1",
  "Nome": "Admin"
}
```

A senha precisa de no minimo 8 caracteres, com digito, maiuscula e minuscula. No boot, a API cria a role `Administrador` e o usuario se o e-mail ainda nao existir. Sem `Admin:Email` e `Admin:Senha`, o seed nao faz nada.

`Jwt:SigningKey` vazio impede a API de subir. Nao commite a chave nem a senha.

## Enums no banco

As colunas de status/lado/slot guardam o nome do enum C# (`Rascunho`, `Pending`, `Winners`, `Team`). A API responde as strings do site (`rascunho`, `pending`, `winners`, `team`). Nao grave as strings da API direto no banco.
