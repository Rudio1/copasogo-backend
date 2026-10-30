# Auth

So login. Nao existe `POST /api/auth/register`.

## POST /api/auth/login

Anonimo.

```json
{ "email": "admin@copasogo.local", "senha": "TroqueEsta1" }
```

200:

```json
{
  "success": true,
  "data": {
    "accessToken": "<jwt>",
    "expiresAt": "2026-10-30T12:00:00.000Z",
    "usuario": {
      "id": "00000000-0000-0000-0000-000000000000",
      "nome": "Admin",
      "email": "admin@copasogo.local"
    }
  },
  "error": null
}
```

O token vale 60 minutos. Claims: `sub` (id), `email`, `name`, `role` = `Administrador`. Issuer `Copasogo`, audience `Copasogo.WebApi`.

Nas rotas de escrita: `Authorization: Bearer {accessToken}`.

401 `UNAUTHORIZED` se e-mail ou senha forem invalidos, ou se a conta estiver bloqueada (5 falhas, 15 minutos). A mensagem de senha errada e a de e-mail inexistente sao a mesma: `E-mail ou senha invalidos.` Conta bloqueada tem mensagem propria.

400 `VALIDATION_ERROR` se `email` ou `senha` vierem vazios. `email` precisa ser um e-mail.

O usuario admin e criado no boot quando `Admin:Email` e `Admin:Senha` estao configurados e as tabelas Identity ja existem. Senha: minimo 8, digito, maiuscula e minuscula.
