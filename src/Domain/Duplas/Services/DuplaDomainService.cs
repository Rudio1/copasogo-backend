using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Domain.Duplas.Services.Interfaces;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Jogadores.Repositories.Interfaces;
using Copasogo.Domain.Partidas;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Partidas.Repositories.Interfaces;
using Copasogo.Domain.Torneio;
using Copasogo.Domain.Torneio.Repositories.Interfaces;

namespace Copasogo.Domain.Duplas.Services;

public sealed class DuplaDomainService(
    IDuplaRepository duplas,
    IJogadorRepository jogadores,
    IPartidaRepository partidas,
    ITorneioRepository torneios) : IDuplaDomainService
{
    public async Task<DuplaConsulta> CriarAsync(Guid torneioId, string nome, Guid jogadorAId, Guid jogadorBId, CancellationToken cancellationToken = default)
    {
        var torneio = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        torneio.GarantirEditavel();

        var times = await duplas.ListarPorTorneioAsync(torneioId, cancellationToken);
        var vagas = ChaveFabrica.Vagas(torneio.QuantidadePartidas);
        if (times.Count >= vagas)
        {
            throw new BusinessException(string.Format(DomainMessages.Dupla.LimiteAtingido, vagas));
        }

        GarantirElencoLivre(times, null, jogadorAId, jogadorBId);
        await GarantirJogadoresAsync(jogadorAId, jogadorBId, cancellationToken);

        var dupla = Dupla.Criar(torneioId, nome, jogadorAId, jogadorBId);
        await duplas.AddAsync(dupla, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(torneioId, cancellationToken);
        ProgressaoChave.Sincronizar(jogos, [.. times, dupla]);
        return await MontarAsync(dupla, cancellationToken);
    }

    public async Task<DuplaConsulta> AtualizarAsync(Guid id, string nome, Guid jogadorAId, Guid jogadorBId, CancellationToken cancellationToken = default)
    {
        var dupla = await ObterObrigatoriaAsync(id, cancellationToken);
        var torneio = await torneios.GetByIdAsync(dupla.TorneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        torneio.GarantirEditavel();

        var times = await duplas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        if (ProgressaoChave.DuplaEntrou(dupla.Id, jogos, times)
            && (dupla.JogadorAId != jogadorAId || dupla.JogadorBId != jogadorBId))
        {
            throw new BusinessException(DomainMessages.Dupla.JaJogou);
        }

        GarantirElencoLivre(times, dupla.Id, jogadorAId, jogadorBId);
        await GarantirJogadoresAsync(jogadorAId, jogadorBId, cancellationToken);

        dupla.Redefinir(nome, jogadorAId, jogadorBId);
        ProgressaoChave.Sincronizar(jogos, times);
        return await MontarAsync(dupla, cancellationToken);
    }

    public async Task<IReadOnlyList<DuplaConsulta>> SortearAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        var torneio = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        torneio.GarantirEditavel();

        var times = (await duplas.ListarPorTorneioAsync(torneioId, cancellationToken)).ToList();
        var vagas = ChaveFabrica.Vagas(torneio.QuantidadePartidas);
        if (times.Count < vagas)
        {
            throw new BusinessException(string.Format(DomainMessages.Dupla.SorteioCedo, vagas));
        }

        if (times.Count > vagas)
        {
            throw new BusinessException(string.Format(DomainMessages.Torneio.ChaveCheia, vagas));
        }

        var jogos = await partidas.ListarPorTorneioAsync(torneioId, cancellationToken);
        if (jogos.Count == 0)
        {
            throw new BusinessException(DomainMessages.Dupla.ChaveVazia);
        }
        if (jogos.Any(jogo => jogo.Status is StatusPartida.Live or StatusPartida.Completed))
        {
            throw new BusinessException(DomainMessages.Dupla.SorteioTarde);
        }

        foreach (var time in times)
        {
            if (time.Seed is int seed)
            {
                DesvincularSeed(jogos, seed);
            }
        }

        Embaralhar(times);
        for (var indice = 0; indice < times.Count; indice++)
        {
            var seed = indice + 1;
            times[indice].DefinirSeed(seed, vagas);
            VincularSeed(jogos, seed, times[indice].Id);
        }

        ProgressaoChave.Sincronizar(jogos, times);
        var consultas = new List<DuplaConsulta>();
        foreach (var time in times.OrderBy(item => item.Seed))
        {
            consultas.Add(await MontarAsync(time, cancellationToken));
        }

        return consultas;
    }

    public async Task<IReadOnlyList<DuplaConsulta>> ResetarChaveAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        var torneio = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        torneio.GarantirEditavel();

        var jogos = await partidas.ListarPorTorneioAsync(torneioId, cancellationToken);
        if (jogos.Count == 0)
        {
            throw new BusinessException(DomainMessages.Dupla.ChaveVazia);
        }

        foreach (var jogo in jogos)
        {
            jogo.Reiniciar();
        }

        var times = (await duplas.ListarPorTorneioAsync(torneioId, cancellationToken)).ToList();
        foreach (var time in times)
        {
            time.LimparSeed();
        }

        ProgressaoChave.Sincronizar(jogos, times);
        var consultas = new List<DuplaConsulta>();
        foreach (var time in times.OrderBy(item => item.Nome))
        {
            consultas.Add(await MontarAsync(time, cancellationToken));
        }

        return consultas;
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dupla = await ObterObrigatoriaAsync(id, cancellationToken);
        var torneio = await torneios.GetByIdAsync(dupla.TorneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        torneio.GarantirEditavel();

        var times = await duplas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        if (ProgressaoChave.DuplaEntrou(dupla.Id, jogos, times))
        {
            throw new BusinessException(DomainMessages.Dupla.JaJogou);
        }

        if (dupla.Seed is int seed)
        {
            DesvincularSeed(jogos, seed);
        }
        duplas.Remove(dupla);
        ProgressaoChave.Sincronizar(jogos, times.Where(time => time.Id != dupla.Id).ToArray());
    }

    public async Task<IReadOnlyList<DuplaConsulta>> ListarAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        _ = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        var times = await duplas.ListarPorTorneioAsync(torneioId, cancellationToken);
        var consultas = new List<DuplaConsulta>();
        foreach (var time in times.OrderBy(time => time.Seed ?? int.MaxValue))
        {
            consultas.Add(await MontarAsync(time, cancellationToken));
        }

        return consultas;
    }

    public async Task<DuplaConsulta> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dupla = await ObterObrigatoriaAsync(id, cancellationToken);
        return await MontarAsync(dupla, cancellationToken);
    }

    private async Task<DuplaConsulta> MontarAsync(Dupla dupla, CancellationToken cancellationToken)
    {
        var times = await duplas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        if (times.All(time => time.Id != dupla.Id))
        {
            times = [.. times, dupla];
        }

        var jogos = await partidas.ListarPorTorneioAsync(dupla.TorneioId, cancellationToken);
        ProgressaoChave.Sincronizar(jogos, times);
        var jogadorA = await jogadores.GetByIdAsync(dupla.JogadorAId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Jogador.NaoEncontrado);
        var jogadorB = await jogadores.GetByIdAsync(dupla.JogadorBId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Jogador.NaoEncontrado);
        var classificacao = ProgressaoChave.Classificar(jogos, times).First(item => item.DuplaId == dupla.Id);
        var caminho = ProgressaoChave.Caminho(dupla.Id, jogos, times);
        return new DuplaConsulta(dupla, jogadorA, jogadorB, classificacao, caminho);
    }

    private async Task<Dupla> ObterObrigatoriaAsync(Guid id, CancellationToken cancellationToken)
    {
        return await duplas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Dupla.NaoEncontrada);
    }

    private async Task GarantirJogadoresAsync(Guid jogadorAId, Guid jogadorBId, CancellationToken cancellationToken)
    {
        _ = await jogadores.GetByIdAsync(jogadorAId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Jogador.NaoEncontrado);
        _ = await jogadores.GetByIdAsync(jogadorBId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Jogador.NaoEncontrado);
    }

    private static void Embaralhar(IList<Dupla> times)
    {
        for (var indice = times.Count - 1; indice > 0; indice--)
        {
            var destino = Random.Shared.Next(indice + 1);
            (times[indice], times[destino]) = (times[destino], times[indice]);
        }
    }

    private static void GarantirElencoLivre(IReadOnlyList<Dupla> times, Guid? ignorarId, Guid jogadorAId, Guid jogadorBId)
    {
        if (times.Any(time => time.Id != ignorarId && (time.PossuiJogador(jogadorAId) || time.PossuiJogador(jogadorBId))))
        {
            throw new BusinessException(DomainMessages.Dupla.JogadorJaNaEdicao);
        }
    }

    private static void VincularSeed(IReadOnlyList<Partida> jogos, int seed, Guid duplaId)
    {
        var (numero, slotA) = ChaveFabrica.SlotDaSeed(seed);
        var jogo = jogos.FirstOrDefault(item => item.Numero == numero)
            ?? throw new NotFoundException(DomainMessages.Partida.NaoEncontrada);
        jogo.VincularDupla(slotA, duplaId);
    }

    private static void DesvincularSeed(IReadOnlyList<Partida> jogos, int seed)
    {
        var (numero, slotA) = ChaveFabrica.SlotDaSeed(seed);
        jogos.FirstOrDefault(item => item.Numero == numero)?.DesvincularDupla(slotA);
    }
}
