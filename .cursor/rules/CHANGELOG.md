# Changelog — Cursor Rules (Copasogo)

Registro de alteracoes nas regras do projeto em `.cursor/rules/`.

Sempre que uma rule for criada, alterada ou removida, **adicione uma entrada no topo** deste arquivo (mais recente primeiro).

---

## [2026-10-30] — Contextos de dominio

**Tipo:** Alteracao

**Motivo:**
Auth, edicoes, jogadores, duplas e partidas passaram a existir. A regra de pastas ainda listava `Chave` e `Transmissao` como contextos futuros.

**Antes:**
`02-folder-structure.mdc` previa `Chave` e `Transmissao`.

**Depois:**
Contextos vigentes: `Auth`, `Torneio`, `Jogadores`, `Duplas`, `Partidas`. Chave fica no dominio de torneio/partida. Transmissao permanece no site.

---

## [2026-10-29] — Bootstrap das rules (DDD / Clean Architecture)

**Tipo:** Criacao

**Motivo:**
Estrutura inicial do backend Copasogo, extraida e adaptada do padrao Agropulse. Sem contextos de negocio agro. Sem features de torneio ainda.

**Depois:**
Rules `01` a `24` (exceto `13`) aplicadas ao namespace `Copasogo.*` e pastas `src/{WebApi,Application,Domain,Infra.Data,Infra.Jobs,Jobs.Web,Resources}`.
