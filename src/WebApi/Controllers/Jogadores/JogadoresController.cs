using Copasogo.Application.Auth.Constants;
using Copasogo.Application.Jogadores.Commands;
using Copasogo.WebApi.Controllers.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Jogadores;

[Route("api/jogadores")]
public sealed class JogadoresController(
    ICriarJogadorHandler criar,
    IAtualizarJogadorHandler atualizar,
    IExcluirJogadorHandler excluir,
    IListarJogadoresHandler listar,
    IObterJogadorHandler obter) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? torneioId, CancellationToken cancellationToken) =>
        OkResponse(await listar.HandleAsync(torneioId, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, [FromQuery] Guid? torneioId, CancellationToken cancellationToken) =>
        OkResponse(await obter.HandleAsync(id, torneioId, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] JogadorBody body, CancellationToken cancellationToken)
    {
        var criado = await criar.HandleAsync(new CriarJogadorCommand(body.Name, body.SteamId), cancellationToken);
        return CreatedResponse($"/api/jogadores/{criado.Id}", criado);
    }

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] JogadorBody body, CancellationToken cancellationToken) =>
        OkResponse(await atualizar.HandleAsync(new AtualizarJogadorCommand(id, body.Name, body.SteamId), cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await excluir.HandleAsync(id, cancellationToken);
        return NoContentResponse();
    }

    public sealed record JogadorBody(string Name, string SteamId);
}
