using Copasogo.Application.Auth.Constants;
using Copasogo.Application.Duplas.Commands;
using Copasogo.WebApi.Controllers.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Duplas;

[Route("api")]
public sealed class DuplasController(
    ICriarDuplaHandler criar,
    IAtualizarDuplaHandler atualizar,
    IExcluirDuplaHandler excluir,
    IListarDuplasHandler listar,
    IObterDuplaHandler obter,
    ISortearChaveHandler sortear,
    IResetarChaveHandler resetar) : ApiControllerBase
{
    [HttpGet("torneios/{torneioId:guid}/duplas")]
    public async Task<IActionResult> Listar(Guid torneioId, CancellationToken cancellationToken) =>
        OkResponse(await listar.HandleAsync(torneioId, cancellationToken));

    [HttpGet("duplas/{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        OkResponse(await obter.HandleAsync(id, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("torneios/{torneioId:guid}/duplas")]
    public async Task<IActionResult> Criar(Guid torneioId, [FromBody] DuplaBody body, CancellationToken cancellationToken)
    {
        var (jogadorA, jogadorB) = Elenco(body);
        var criado = await criar.HandleAsync(new SalvarDuplaCommand(null, torneioId, body.Name, jogadorA, jogadorB), cancellationToken);
        return CreatedResponse($"/api/duplas/{criado.Id}", criado);
    }

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPut("duplas/{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] DuplaBody body, CancellationToken cancellationToken)
    {
        var (jogadorA, jogadorB) = Elenco(body);
        return OkResponse(await atualizar.HandleAsync(new SalvarDuplaCommand(id, Guid.Empty, body.Name, jogadorA, jogadorB), cancellationToken));
    }

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpDelete("duplas/{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await excluir.HandleAsync(id, cancellationToken);
        return NoContentResponse();
    }

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("torneios/{torneioId:guid}/duplas/sortear")]
    public async Task<IActionResult> Sortear(Guid torneioId, CancellationToken cancellationToken) =>
        OkResponse(await sortear.HandleAsync(torneioId, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("torneios/{torneioId:guid}/duplas/resetar")]
    public async Task<IActionResult> Resetar(Guid torneioId, CancellationToken cancellationToken) =>
        OkResponse(await resetar.HandleAsync(torneioId, cancellationToken));

    public sealed record DuplaBody(string Name, Guid[] PlayerIds);

    private static (Guid A, Guid B) Elenco(DuplaBody body)
    {
        if (body.PlayerIds is not { Length: 2 })
        {
            throw new Copasogo.Domain.Generic.Validations.ValidationException("A dupla precisa de dois jogadores.");
        }

        return (body.PlayerIds[0], body.PlayerIds[1]);
    }
}
