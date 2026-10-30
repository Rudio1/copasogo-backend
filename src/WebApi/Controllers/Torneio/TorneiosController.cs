using Copasogo.Application.Auth.Constants;
using Copasogo.Application.Torneio.Commands;
using Copasogo.WebApi.Controllers.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Torneio;

[Route("api/torneios")]
public sealed class TorneiosController(
    ICriarTorneioHandler criar,
    IAtualizarTorneioHandler atualizar,
    IIniciarTorneioHandler iniciar,
    IEncerrarTorneioHandler encerrar,
    IExcluirTorneioHandler excluir,
    IObterTorneioHandler obter,
    IObterTorneioAtualHandler obterAtual,
    IListarTorneiosHandler listar) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken) =>
        OkResponse(await listar.HandleAsync(cancellationToken));

    [HttpGet("atual")]
    public async Task<IActionResult> Atual(CancellationToken cancellationToken) =>
        OkResponse(await obterAtual.HandleAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        OkResponse(await obter.HandleAsync(id, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTorneioCommand command, CancellationToken cancellationToken)
    {
        var criado = await criar.HandleAsync(command, cancellationToken);
        return CreatedResponse($"/api/torneios/{criado.Id}", criado);
    }

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarTorneioBody body, CancellationToken cancellationToken) =>
        OkResponse(await atualizar.HandleAsync(new AtualizarTorneioCommand(id, body.Name, body.EntryFeeCents, body.PrizePoolCents, body.MatchCount), cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("{id:guid}/iniciar")]
    public async Task<IActionResult> Iniciar(Guid id, CancellationToken cancellationToken) =>
        OkResponse(await iniciar.HandleAsync(new TorneioIdCommand(id), cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("{id:guid}/encerrar")]
    public async Task<IActionResult> Encerrar(Guid id, CancellationToken cancellationToken) =>
        OkResponse(await encerrar.HandleAsync(new TorneioIdCommand(id), cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await excluir.HandleAsync(new TorneioIdCommand(id), cancellationToken);
        return NoContentResponse();
    }

    public sealed record AtualizarTorneioBody(string Name, int EntryFeeCents, int PrizePoolCents, int MatchCount);
}
