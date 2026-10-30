using Copasogo.Application.Auth.Constants;
using Copasogo.Application.Partidas.Commands;
using Copasogo.WebApi.Controllers.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Partidas;

[Route("api/torneios/{torneioId:guid}/partidas")]
public sealed class PartidasController(
    IListarPartidasHandler listar,
    IObterPartidaHandler obter,
    IAgendarPartidaHandler agendar,
    IMarcarAoVivoHandler aoVivo,
    IRegistrarResultadoHandler resultado,
    ICancelarPartidaHandler cancelar) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(Guid torneioId, [FromQuery] string? roundId, [FromQuery] string? status, [FromQuery] string? side, CancellationToken cancellationToken) =>
        OkResponse(await listar.HandleAsync(new FiltroPartidas(torneioId, roundId, status, side), cancellationToken));

    [HttpGet("{numero:int}")]
    public async Task<IActionResult> Obter(Guid torneioId, int numero, CancellationToken cancellationToken) =>
        OkResponse(await obter.HandleAsync(torneioId, numero, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPatch("{numero:int}")]
    public async Task<IActionResult> Agendar(Guid torneioId, int numero, [FromBody] AgendarBody body, CancellationToken cancellationToken) =>
        OkResponse(await agendar.HandleAsync(new AgendarPartidaCommand(torneioId, numero, body.ScheduledAt), cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("{numero:int}/ao-vivo")]
    public async Task<IActionResult> AoVivo(Guid torneioId, int numero, CancellationToken cancellationToken) =>
        OkResponse(await aoVivo.HandleAsync(torneioId, numero, cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("{numero:int}/resultado")]
    public async Task<IActionResult> Resultado(Guid torneioId, int numero, [FromBody] ResultadoBody body, CancellationToken cancellationToken) =>
        OkResponse(await resultado.HandleAsync(
            new RegistrarResultadoCommand(torneioId, numero, body.WinnerTeamId, body.Score?.A, body.Score?.B),
            cancellationToken));

    [Authorize(Roles = AuthRoles.Administrador)]
    [HttpPost("{numero:int}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid torneioId, int numero, CancellationToken cancellationToken) =>
        OkResponse(await cancelar.HandleAsync(torneioId, numero, cancellationToken));

    public sealed record AgendarBody(DateTime? ScheduledAt);
    public sealed record ScoreBody(int A, int B);
    public sealed record ResultadoBody(Guid WinnerTeamId, ScoreBody? Score);
}
