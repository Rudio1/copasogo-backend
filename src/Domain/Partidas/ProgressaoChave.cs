using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Torneio;

namespace Copasogo.Domain.Partidas;

public static class ProgressaoChave
{
    public static void Sincronizar(IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        foreach (var partida in partidas.OrderBy(item => item.Numero))
        {
            partida.AplicarStatusCalculado(ProximoStatus(partida, partidas, duplas));
        }
    }

    public static void ValidarPlacar(int placarA, int placarB, bool vencedorEhA, int numero)
    {
        if (placarA < 0 || placarB < 0)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.PlacarInvalido, numero));
        }

        var vencedor = vencedorEhA ? placarA : placarB;
        var perdedor = vencedorEhA ? placarB : placarA;
        if (vencedor <= perdedor)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.PlacarNaoConfirma, numero));
        }
    }

    public static (Dupla? A, Dupla? B) Participantes(Partida partida, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        return (Resolver(partida, true, partidas, duplas), Resolver(partida, false, partidas, duplas));
    }

    public static bool DuplaEntrou(Guid duplaId, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        foreach (var partida in partidas)
        {
            if (partida.Status is not (StatusPartida.Live or StatusPartida.Completed))
            {
                continue;
            }

            if (partida.VencedorDuplaId == duplaId || partida.SlotADuplaId == duplaId || partida.SlotBDuplaId == duplaId)
            {
                return true;
            }

            var (a, b) = Participantes(partida, partidas, duplas);
            if (a?.Id == duplaId || b?.Id == duplaId)
            {
                return true;
            }
        }

        return false;
    }

    public static IReadOnlyList<ClassificacaoDupla> Classificar(IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var campeao = CampeaoId(partidas, duplas);
        var vice = ViceId(partidas, duplas, campeao);
        return duplas
            .OrderBy(dupla => dupla.Seed ?? int.MaxValue)
            .Select(dupla =>
            {
                var derrotas = Derrotas(dupla.Id, partidas, duplas);
                return new ClassificacaoDupla(dupla.Id, Estado(dupla.Id, derrotas, campeao, vice), derrotas);
            })
            .ToArray();
    }

    public static Guid? CampeaoId(IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas) =>
        Decisiva(partidas)?.VencedorDuplaId;

    public static CaminhoDupla Caminho(Guid duplaId, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var passado = new List<int>();
        var proximo = new SortedSet<int>();

        foreach (var partida in partidas)
        {
            if (!Participa(duplaId, partida, partidas, duplas))
            {
                continue;
            }

            if (partida.Status == StatusPartida.Completed)
            {
                passado.Add(partida.Numero);
                continue;
            }

            if (partida.Status == StatusPartida.Cancelled)
            {
                continue;
            }

            proximo.Add(partida.Numero);
            foreach (var candidata in partidas)
            {
                if (candidata.Status is StatusPartida.Completed or StatusPartida.Cancelled)
                {
                    continue;
                }

                if (Alimenta(candidata, partida.Numero))
                {
                    proximo.Add(candidata.Numero);
                }
            }
        }

        passado.Sort();
        return new CaminhoDupla(passado, proximo.ToArray());
    }

    public static Partida? Encontrar(IReadOnlyList<Partida> partidas, int numero) =>
        partidas.FirstOrDefault(partida => partida.Numero == numero);

    private static Partida? JogoDaRodada(IReadOnlyList<Partida> partidas, string rodadaId) =>
        partidas.FirstOrDefault(partida => partida.RodadaId == rodadaId);

    private static Partida? FinalDosVencedores(IReadOnlyList<Partida> partidas) =>
        partidas.Where(partida => partida.Lado == LadoChave.Winners).OrderByDescending(partida => partida.Numero).FirstOrDefault();

    private static StatusPartida ProximoStatus(Partida partida, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        if (partida.Status is StatusPartida.Completed or StatusPartida.Live or StatusPartida.Cancelled)
        {
            return partida.Status;
        }

        if (partida.RodadaId == "gf-reset")
        {
            return StatusReset(partidas, duplas);
        }

        var (a, b) = Participantes(partida, partidas, duplas);
        return a is not null && b is not null ? StatusPartida.Upcoming : StatusPartida.Pending;
    }

    private static StatusPartida StatusReset(IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var grande = JogoDaRodada(partidas, "gf");
        var final = FinalDosVencedores(partidas);
        if (grande is null || final is null || grande.Status != StatusPartida.Completed)
        {
            return StatusPartida.Pending;
        }

        var campeaoVencedores = ResolverOrigem(TipoSlot.WinnerOf, final.Numero, partidas, duplas);
        return grande.VencedorDuplaId == campeaoVencedores?.Id ? StatusPartida.Cancelled : StatusPartida.Upcoming;
    }

    private static Dupla? Resolver(Partida partida, bool slotA, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var tipo = slotA ? partida.SlotATipo : partida.SlotBTipo;
        var duplaId = slotA ? partida.SlotADuplaId : partida.SlotBDuplaId;
        var seed = slotA ? partida.SlotASeed : partida.SlotBSeed;
        var origem = slotA ? partida.SlotAOrigem : partida.SlotBOrigem;

        if (tipo == TipoSlot.Team)
        {
            if (duplaId is Guid id)
            {
                return duplas.FirstOrDefault(dupla => dupla.Id == id);
            }

            return seed is int valor ? duplas.FirstOrDefault(dupla => dupla.Seed == valor) : null;
        }

        return origem is int numero ? ResolverOrigem(tipo, numero, partidas, duplas) : null;
    }

    private static Dupla? ResolverOrigem(TipoSlot tipo, int numero, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var origem = Encontrar(partidas, numero);
        if (origem is null || origem.Status != StatusPartida.Completed || origem.VencedorDuplaId is not Guid vencedorId)
        {
            return null;
        }

        if (tipo == TipoSlot.WinnerOf)
        {
            return duplas.FirstOrDefault(dupla => dupla.Id == vencedorId);
        }

        var (a, b) = Participantes(origem, partidas, duplas);
        return a?.Id == vencedorId ? b : a;
    }

    private static bool Participa(Guid duplaId, Partida partida, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var (a, b) = Participantes(partida, partidas, duplas);
        return a?.Id == duplaId || b?.Id == duplaId;
    }

    private static bool Alimenta(Partida partida, int origem)
    {
        return (partida.SlotATipo is TipoSlot.WinnerOf or TipoSlot.LoserOf && partida.SlotAOrigem == origem)
            || (partida.SlotBTipo is TipoSlot.WinnerOf or TipoSlot.LoserOf && partida.SlotBOrigem == origem);
    }

    private static int Derrotas(Guid duplaId, IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas)
    {
        var derrotas = 0;
        foreach (var partida in partidas)
        {
            if (partida.Status != StatusPartida.Completed || partida.VencedorDuplaId == duplaId)
            {
                continue;
            }

            if (Participa(duplaId, partida, partidas, duplas))
            {
                derrotas++;
            }
        }

        if (derrotas > 2)
        {
            throw new BusinessException(DomainMessages.Partida.DuplaComMaisDeDuasDerrotas);
        }

        return derrotas;
    }

    private static Partida? Decisiva(IReadOnlyList<Partida> partidas)
    {
        var reset = JogoDaRodada(partidas, "gf-reset");
        var grande = JogoDaRodada(partidas, "gf");
        if (reset?.Status == StatusPartida.Completed)
        {
            return reset;
        }

        if (grande?.Status == StatusPartida.Completed && reset?.Status == StatusPartida.Cancelled)
        {
            return grande;
        }

        return null;
    }

    private static Guid? ViceId(IReadOnlyList<Partida> partidas, IReadOnlyList<Dupla> duplas, Guid? campeaoId)
    {
        var decisiva = Decisiva(partidas);
        if (decisiva?.VencedorDuplaId is null || campeaoId is null)
        {
            return null;
        }

        var (a, b) = Participantes(decisiva, partidas, duplas);
        var outro = a?.Id == campeaoId ? b : a;
        return outro?.Id;
    }

    private static string Estado(Guid duplaId, int derrotas, Guid? campeaoId, Guid? viceId)
    {
        if (duplaId == campeaoId)
        {
            return "champion";
        }

        if (duplaId == viceId)
        {
            return "finalist";
        }

        return derrotas switch
        {
            2 => "eliminated",
            1 => "losers",
            _ => "winners"
        };
    }
}
