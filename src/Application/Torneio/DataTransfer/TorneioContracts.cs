using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Partidas;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Torneio;
using StatusTorneio = Copasogo.Domain.Torneio.StatusTorneio;

namespace Copasogo.Application.Torneio.DataTransfer;

public sealed record PlayerResponse(string Id, string Name, string SteamId)
{
    public static PlayerResponse From(Jogador jogador) => new(jogador.Id.ToString(), jogador.Nome, jogador.SteamId);
}

public sealed record TeamResponse(string Id, string Name, int? Seed, IReadOnlyList<string> PlayerIds)
{
    public static TeamResponse From(Dupla dupla) =>
        new(dupla.Id.ToString(), dupla.Nome, dupla.Seed, [dupla.JogadorAId.ToString(), dupla.JogadorBId.ToString()]);
}

public sealed record RoundResponse(string Id, string Side, string? Stage, string Name, IReadOnlyList<int> MatchIds);

public sealed record SlotResponse(string Type, string? TeamId, int? MatchId);

public sealed record ScoreResponse(int A, int B);

public sealed record PartidaResponse(
    int Id,
    string TorneioId,
    string Side,
    string RoundId,
    string RoundName,
    SlotResponse SlotA,
    SlotResponse SlotB,
    string Status,
    DateTime? ScheduledAt,
    ScoreResponse? Score,
    string? WinnerTeamId)
{
    public static PartidaResponse From(Partida partida, int quantidadePartidas)
    {
        var rodada = ChaveFabrica.RodadaDoJogo(partida.Numero, quantidadePartidas);
        return new(
            partida.Numero,
            partida.TorneioId.ToString(),
            Lado(partida.Lado),
            partida.RodadaId,
            rodada.Nome,
            Slot(partida, true),
            Slot(partida, false),
            partida.StatusApi(),
            partida.AgendadaEm,
            partida.PlacarA is int a && partida.PlacarB is int b ? new ScoreResponse(a, b) : null,
            partida.VencedorDuplaId?.ToString());
    }

    private static SlotResponse Slot(Partida partida, bool slotA)
    {
        var tipo = slotA ? partida.SlotATipo : partida.SlotBTipo;
        if (tipo == TipoSlot.Team)
        {
            return new SlotResponse("team", (slotA ? partida.SlotADuplaId : partida.SlotBDuplaId)?.ToString(), null);
        }

        var origem = slotA ? partida.SlotAOrigem : partida.SlotBOrigem;
        return new SlotResponse(tipo == TipoSlot.WinnerOf ? "winner_of" : "loser_of", null, origem);
    }

    private static string Lado(LadoChave lado) => lado switch
    {
        LadoChave.Losers => "losers",
        LadoChave.GrandFinal => "grand_final",
        _ => "winners"
    };
}

public sealed record StandingResponse(string TeamId, string State, int Losses);

public sealed record PathResponse(IReadOnlyList<int> Past, IReadOnlyList<int> Next);

public sealed record TorneioResponse(
    string Id,
    string Slug,
    string Name,
    int Edicao,
    string Status,
    int EntryFeeCents,
    int PrizePoolCents,
    int MatchCount,
    DateTime? IniciadoEm,
    DateTime? EncerradoEm,
    IReadOnlyList<PlayerResponse> Players,
    IReadOnlyList<TeamResponse> Teams,
    IReadOnlyList<RoundResponse> Rounds,
    IReadOnlyList<PartidaResponse> Matches,
    IReadOnlyList<StandingResponse> Standings,
    TeamResponse? Champion,
    IReadOnlyDictionary<string, PathResponse> Paths)
{
    public static TorneioResponse From(VisaoTorneio visao)
    {
        var times = visao.Duplas.Select(TeamResponse.From).ToArray();
        var quantidade = visao.Torneio.QuantidadePartidas is 3 or 7 or 15 or 31 ? visao.Torneio.QuantidadePartidas : 31;
        return new(
            visao.Torneio.Id.ToString(),
            visao.Torneio.Slug,
            visao.Torneio.Nome,
            visao.Torneio.Edicao,
            StatusDe(visao.Torneio.Status),
            visao.Torneio.EntryFeeCents,
            visao.Torneio.PrizePoolCents,
            quantidade,
            visao.Torneio.IniciadoEm,
            visao.Torneio.EncerradoEm,
            visao.Jogadores.Select(PlayerResponse.From).ToArray(),
            times,
            ChaveFabrica.RodadasDe(quantidade).Select(rodada => new RoundResponse(rodada.Id, Lado(rodada.Lado), rodada.Stage, rodada.Nome, rodada.MatchIds)).ToArray(),
            visao.Partidas.OrderBy(partida => partida.Numero).Select(partida => PartidaResponse.From(partida, quantidade)).ToArray(),
            visao.Classificacao.Select(item => new StandingResponse(item.DuplaId.ToString(), item.Estado, item.Derrotas)).ToArray(),
            times.FirstOrDefault(time => time.Id == visao.CampeaoDuplaId?.ToString()),
            visao.Caminhos.ToDictionary(item => item.Key.ToString(), item => new PathResponse(item.Value.Passado, item.Value.Proximo)));
    }

    public static string StatusDe(StatusTorneio status) => status switch
    {
        StatusTorneio.EmAndamento => "em_andamento",
        StatusTorneio.Encerrado => "encerrado",
        _ => "rascunho"
    };

    private static string Lado(LadoChave lado) => lado switch
    {
        LadoChave.Losers => "losers",
        LadoChave.GrandFinal => "grand_final",
        _ => "winners"
    };
}

public sealed record TorneioResumoResponse(string Id, string Slug, string Name, int Edicao, string Status, string? ChampionId, string? ChampionName)
{
    public static TorneioResumoResponse From(ResumoTorneio resumo) =>
        new(
            resumo.Torneio.Id.ToString(),
            resumo.Torneio.Slug,
            resumo.Torneio.Nome,
            resumo.Torneio.Edicao,
            TorneioResponse.StatusDe(resumo.Torneio.Status),
            resumo.CampeaoDuplaId?.ToString(),
            resumo.CampeaoNome);
}
