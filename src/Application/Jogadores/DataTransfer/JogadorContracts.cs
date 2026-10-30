using Copasogo.Application.Torneio.DataTransfer;
using Copasogo.Domain.Jogadores.Services.Interfaces;

namespace Copasogo.Application.Jogadores.DataTransfer;

public sealed record JogadorResponse(
    string Id,
    string Name,
    string SteamId,
    string? TeamId,
    string? TeamName,
    PlayerResponse? Partner,
    string? Standing)
{
    public static JogadorResponse From(JogadorConsulta consulta) =>
        new(
            consulta.Jogador.Id.ToString(),
            consulta.Jogador.Nome,
            consulta.Jogador.SteamId,
            consulta.Dupla?.Id.ToString(),
            consulta.Dupla?.Nome,
            consulta.Parceiro is null ? null : PlayerResponse.From(consulta.Parceiro),
            consulta.Classificacao?.Estado);
}
