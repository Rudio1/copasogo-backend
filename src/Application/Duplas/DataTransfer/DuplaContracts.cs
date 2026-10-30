using Copasogo.Application.Torneio.DataTransfer;
using Copasogo.Domain.Duplas.Services.Interfaces;

namespace Copasogo.Application.Duplas.DataTransfer;

public sealed record DuplaResponse(
    string Id,
    string TorneioId,
    string Name,
    int? Seed,
    IReadOnlyList<string> PlayerIds,
    IReadOnlyList<PlayerResponse> Players,
    StandingResponse Standing,
    PathResponse Path)
{
    public static DuplaResponse From(DuplaConsulta consulta) =>
        new(
            consulta.Dupla.Id.ToString(),
            consulta.Dupla.TorneioId.ToString(),
            consulta.Dupla.Nome,
            consulta.Dupla.Seed,
            [consulta.Dupla.JogadorAId.ToString(), consulta.Dupla.JogadorBId.ToString()],
            [PlayerResponse.From(consulta.JogadorA), PlayerResponse.From(consulta.JogadorB)],
            new StandingResponse(consulta.Classificacao.DuplaId.ToString(), consulta.Classificacao.Estado, consulta.Classificacao.Derrotas),
            new PathResponse(consulta.Caminho.Passado, consulta.Caminho.Proximo));
}
