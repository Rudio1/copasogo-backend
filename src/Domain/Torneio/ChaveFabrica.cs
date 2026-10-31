using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Partidas;
using Copasogo.Domain.Partidas.Entities;

namespace Copasogo.Domain.Torneio;

public sealed record RodadaDefinicao(string Id, LadoChave Lado, string? Stage, string Nome, IReadOnlyList<int> MatchIds);

public static class ChaveFabrica
{
    private static readonly int[] Quantidades = [3, 7, 15, 31];

    private static readonly Dictionary<int, DefinicaoChave> Definicoes = new()
    {
        [3] = Montar(2),
        [7] = Montar(4),
        [15] = Montar(8),
        [31] = Montar(16)
    };

    public static IReadOnlyList<RodadaDefinicao> Rodadas => Definicoes[31].Rodadas;

    public static void GarantirQuantidade(int quantidade)
    {
        if (!Quantidades.Contains(quantidade))
        {
            throw new BusinessException(DomainMessages.Torneio.QuantidadeInvalida);
        }
    }

    public static int Vagas(int quantidade)
    {
        GarantirQuantidade(quantidade);
        return (quantidade + 1) / 2;
    }

    public static IReadOnlyList<RodadaDefinicao> RodadasDe(int quantidade) => Definicao(quantidade).Rodadas;

    public static RodadaDefinicao RodadaDoJogo(int numero) => RodadaDoJogo(numero, 31);

    public static RodadaDefinicao RodadaDoJogo(int numero, int quantidade) =>
        Definicao(quantidade).Rodadas.First(rodada => rodada.MatchIds.Contains(numero));

    public static IReadOnlyList<Partida> CriarPartidas(Guid torneioId) => CriarPartidas(torneioId, 31);

    public static IReadOnlyList<Partida> CriarPartidas(Guid torneioId, int quantidade)
    {
        var definicao = Definicao(quantidade);
        return Enumerable.Range(1, quantidade)
            .Select(numero =>
            {
                var rodada = definicao.Rodadas.First(item => item.MatchIds.Contains(numero));
                var (a, b) = definicao.Slots[numero];
                return Partida.Criar(
                    torneioId,
                    numero,
                    rodada.Lado,
                    rodada.Id,
                    a.Tipo,
                    a.Seed,
                    a.Origem,
                    b.Tipo,
                    b.Seed,
                    b.Origem);
            })
            .ToArray();
    }

    public static (int Numero, bool SlotA) SlotDaSeed(int seed)
    {
        var index = seed - 1;
        return (index / 2 + 1, index % 2 == 0);
    }

    private static DefinicaoChave Definicao(int quantidade)
    {
        GarantirQuantidade(quantidade);
        return Definicoes[quantidade];
    }

    private static DefinicaoChave Montar(int duplas)
    {
        var slots = new Dictionary<int, (SlotSpec A, SlotSpec B)>();
        var rodadas = new List<RodadaDefinicao>();
        var numero = 1;
        var vencedores = new List<int[]>();
        var tamanho = duplas / 2;
        var rodadaVencedores = 1;

        while (tamanho >= 1)
        {
            var ids = new int[tamanho];
            for (var indice = 0; indice < tamanho; indice++)
            {
                ids[indice] = numero++;
                if (rodadaVencedores == 1)
                {
                    slots[ids[indice]] = (SlotSpec.Team(indice * 2 + 1), SlotSpec.Team(indice * 2 + 2));
                }
                else
                {
                    var anterior = vencedores[rodadaVencedores - 2];
                    slots[ids[indice]] = (SlotSpec.Winner(anterior[indice * 2]), SlotSpec.Winner(anterior[indice * 2 + 1]));
                }
            }

            rodadas.Add(new RodadaDefinicao(
                $"wb-r{rodadaVencedores}",
                LadoChave.Winners,
                $"Rodada {rodadaVencedores}",
                NomeVencedores(tamanho * 2),
                ids));
            vencedores.Add(ids);
            tamanho /= 2;
            rodadaVencedores++;
        }

        var perdedores = Array.Empty<int>();
        if (duplas >= 4)
        {
            var rodadaPerdedores = 1;
            var abertura = vencedores[0];
            var pares = new int[abertura.Length / 2];
            for (var indice = 0; indice < pares.Length; indice++)
            {
                pares[indice] = numero++;
                slots[pares[indice]] = (SlotSpec.Loser(abertura[indice * 2]), SlotSpec.Loser(abertura[indice * 2 + 1]));
            }

            AdicionarPerdedores(rodadas, pares, rodadaPerdedores, false);
            rodadaPerdedores++;
            perdedores = pares;

            for (var rodada = 1; rodada < vencedores.Count; rodada++)
            {
                var queda = vencedores[rodada];
                var ultima = rodada == vencedores.Count - 1;
                var encontro = new int[queda.Length];
                for (var indice = 0; indice < queda.Length; indice++)
                {
                    encontro[indice] = numero++;
                    // A primeira queda entra invertida (com 16 duplas: 12→20, 11→21, 10→22, 9→23)
                    // para que duplas da mesma metade da abertura não se reencontrem logo.
                    var origem = rodada == 1 ? queda[queda.Length - 1 - indice] : queda[indice];
                    slots[encontro[indice]] = (SlotSpec.Winner(perdedores[indice]), SlotSpec.Loser(origem));
                }

                if (ultima)
                {
                    AdicionarPerdedores(rodadas, encontro, rodadaPerdedores, true);
                    perdedores = encontro;
                    break;
                }

                AdicionarPerdedores(rodadas, encontro, rodadaPerdedores, false);
                rodadaPerdedores++;
                var interno = new int[encontro.Length / 2];
                for (var indice = 0; indice < interno.Length; indice++)
                {
                    interno[indice] = numero++;
                    slots[interno[indice]] = (SlotSpec.Winner(encontro[indice * 2]), SlotSpec.Winner(encontro[indice * 2 + 1]));
                }

                AdicionarPerdedores(rodadas, interno, rodadaPerdedores, false);
                rodadaPerdedores++;
                perdedores = interno;
            }
        }

        var finalVencedores = vencedores[^1][0];
        var grande = numero++;
        var reset = numero;
        if (duplas == 2)
        {
            slots[grande] = (SlotSpec.Winner(finalVencedores), SlotSpec.Loser(finalVencedores));
            slots[reset] = (SlotSpec.Winner(finalVencedores), SlotSpec.Winner(grande));
        }
        else
        {
            var finalPerdedores = perdedores[0];
            slots[grande] = (SlotSpec.Winner(finalVencedores), SlotSpec.Winner(finalPerdedores));
            slots[reset] = (SlotSpec.Winner(finalVencedores), SlotSpec.Winner(finalPerdedores));
        }

        rodadas.Add(new RodadaDefinicao("gf", LadoChave.GrandFinal, null, "Grande final", [grande]));
        rodadas.Add(new RodadaDefinicao("gf-reset", LadoChave.GrandFinal, null, "Reset da final", [reset]));
        return new DefinicaoChave(rodadas, slots);
    }

    private static void AdicionarPerdedores(List<RodadaDefinicao> rodadas, int[] ids, int indice, bool ultima)
    {
        rodadas.Add(new RodadaDefinicao(
            $"lb-r{indice}",
            LadoChave.Losers,
            $"Rodada {indice}",
            ultima ? "Final dos perdedores" : $"Rodada {indice}",
            ids));
    }

    private static string NomeVencedores(int entrantes) => entrantes switch
    {
        >= 16 => "Oitavas de final",
        8 => "Quartas de final",
        4 => "Semifinais",
        _ => "Final dos vencedores"
    };

    private sealed record DefinicaoChave(IReadOnlyList<RodadaDefinicao> Rodadas, Dictionary<int, (SlotSpec A, SlotSpec B)> Slots);

    private readonly record struct SlotSpec(TipoSlot Tipo, int? Seed, int? Origem)
    {
        public static SlotSpec Team(int seed) => new(TipoSlot.Team, seed, null);
        public static SlotSpec Winner(int origem) => new(TipoSlot.WinnerOf, null, origem);
        public static SlotSpec Loser(int origem) => new(TipoSlot.LoserOf, null, origem);
    }
}
