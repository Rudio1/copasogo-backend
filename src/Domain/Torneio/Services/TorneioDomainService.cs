using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Jogadores.Repositories.Interfaces;
using Copasogo.Domain.Partidas;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Partidas.Repositories.Interfaces;
using Copasogo.Domain.Torneio.Entities;
using Copasogo.Domain.Torneio.Repositories.Interfaces;
using Copasogo.Domain.Torneio.Services.Interfaces;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Domain.Torneio.Services;

public sealed class TorneioDomainService(
    ITorneioRepository torneios,
    IPartidaRepository partidas,
    IDuplaRepository duplas,
    IJogadorRepository jogadores) : ITorneioDomainService
{
    public async Task<VisaoTorneio> CriarAsync(string nome, string? slug, int edicao, int entryFeeCents, int prizePoolCents, int quantidadePartidas, CancellationToken cancellationToken = default)
    {
        var torneio = TorneioEntity.Criar(nome, slug, edicao, entryFeeCents, prizePoolCents, quantidadePartidas);
        if (await torneios.ExisteEdicaoAsync(torneio.Edicao, cancellationToken: cancellationToken))
        {
            throw new BusinessException(DomainMessages.Torneio.EdicaoJaExiste);
        }

        if (await torneios.ExisteSlugAsync(torneio.Slug, cancellationToken: cancellationToken))
        {
            throw new BusinessException(DomainMessages.Torneio.SlugJaExiste);
        }

        await torneios.AddAsync(torneio, cancellationToken);
        var jogosNovos = ChaveFabrica.CriarPartidas(torneio.Id, torneio.QuantidadePartidas).ToArray();
        await partidas.AddRangeAsync(jogosNovos, cancellationToken);
        ProgressaoChave.Sincronizar(jogosNovos, []);
        return new VisaoTorneio(torneio, [], [], jogosNovos, [], null, new Dictionary<Guid, CaminhoDupla>());
    }

    public async Task<VisaoTorneio> AtualizarAsync(Guid id, string nome, int entryFeeCents, int prizePoolCents, int quantidadePartidas, CancellationToken cancellationToken = default)
    {
        var torneio = await ObterObrigatorioAsync(id, cancellationToken);
        ChaveFabrica.GarantirQuantidade(quantidadePartidas);
        var jogos = (await partidas.ListarPorTorneioAsync(id, cancellationToken)).ToArray();
        var precisaGerar = jogos.Length != quantidadePartidas;
        if (precisaGerar)
        {
            if (jogos.Any(jogo => jogo.Status is StatusPartida.Live or StatusPartida.Completed))
            {
                throw new BusinessException(DomainMessages.Torneio.ChaveTravada);
            }

            var times = (await duplas.ListarPorTorneioAsync(id, cancellationToken)).ToArray();
            var vagas = ChaveFabrica.Vagas(quantidadePartidas);
            if (times.Length > vagas)
            {
                throw new BusinessException(string.Format(DomainMessages.Torneio.ChaveCheia, vagas));
            }

            if (times.Any(time => time.Seed is int seed && seed > vagas))
            {
                throw new BusinessException(DomainMessages.Torneio.PosicaoAcima);
            }

            foreach (var jogo in jogos)
            {
                partidas.Remove(jogo);
            }

            var novos = ChaveFabrica.CriarPartidas(torneio.Id, quantidadePartidas).ToArray();
            await partidas.AddRangeAsync(novos, cancellationToken);
            foreach (var time in times)
            {
                if (time.Seed is int seed)
                {
                    var (numero, slotA) = ChaveFabrica.SlotDaSeed(seed);
                    novos.First(jogo => jogo.Numero == numero).VincularDupla(slotA, time.Id);
                }
            }

            torneio.AtualizarMeta(nome, entryFeeCents, prizePoolCents, quantidadePartidas);
            return await ComporAsync(torneio, times, novos, cancellationToken);
        }

        torneio.AtualizarMeta(nome, entryFeeCents, prizePoolCents, quantidadePartidas);
        return await MontarAsync(torneio, cancellationToken);
    }

    public async Task<VisaoTorneio> IniciarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var torneio = await ObterObrigatorioAsync(id, cancellationToken);
        if (await torneios.ObterEmAndamentoAsync(cancellationToken) is { } atual && atual.Id != torneio.Id)
        {
            throw new BusinessException(DomainMessages.Torneio.JaEmAndamento);
        }

        torneio.Iniciar();
        return await MontarAsync(torneio, cancellationToken);
    }

    public async Task<VisaoTorneio> EncerrarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var torneio = await ObterObrigatorioAsync(id, cancellationToken);
        torneio.Encerrar();
        return await MontarAsync(torneio, cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var torneio = await ObterObrigatorioAsync(id, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(id, cancellationToken);
        var times = await duplas.ListarPorTorneioAsync(id, cancellationToken);
        var travada = jogos.Any(jogo => jogo.Status is StatusPartida.Live or StatusPartida.Completed);
        torneio.GarantirPodeExcluir(travada);

        foreach (var time in times)
        {
            duplas.Remove(time);
        }

        foreach (var jogo in jogos)
        {
            partidas.Remove(jogo);
        }

        torneios.Remove(torneio);
    }

    public async Task<VisaoTorneio> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var torneio = await ObterObrigatorioAsync(id, cancellationToken);
        return await MontarAsync(torneio, cancellationToken);
    }

    public async Task<VisaoTorneio> ObterAtualAsync(CancellationToken cancellationToken = default)
    {
        var torneio = await torneios.ObterAtualAsync(cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NenhumDisponivel);
        return await MontarAsync(torneio, cancellationToken);
    }

    public async Task<IReadOnlyList<ResumoTorneio>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var lista = await torneios.ListarAsync(cancellationToken);
        var todosJogos = await partidas.ListarTodasAsync(cancellationToken);
        var todosTimes = await duplas.ListarTodasAsync(cancellationToken);

        return lista
            .OrderByDescending(item => item.Edicao)
            .Select(item =>
            {
                var jogos = todosJogos.Where(jogo => jogo.TorneioId == item.Id).ToArray();
                var times = todosTimes.Where(time => time.TorneioId == item.Id).ToArray();
                ProgressaoChave.Sincronizar(jogos, times);
                var campeaoId = ProgressaoChave.CampeaoId(jogos, times);
                var campeao = times.FirstOrDefault(time => time.Id == campeaoId);
                return new ResumoTorneio(item, campeaoId, campeao?.Nome);
            })
            .ToArray();
    }

    private async Task<TorneioEntity> ObterObrigatorioAsync(Guid id, CancellationToken cancellationToken)
    {
        return await torneios.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
    }

    private async Task<VisaoTorneio> MontarAsync(TorneioEntity torneio, CancellationToken cancellationToken)
    {
        var times = (await duplas.ListarPorTorneioAsync(torneio.Id, cancellationToken)).OrderBy(time => time.Seed ?? int.MaxValue).ToArray();
        var jogos = (await partidas.ListarPorTorneioAsync(torneio.Id, cancellationToken)).OrderBy(jogo => jogo.Numero).ToArray();
        return await ComporAsync(torneio, times, jogos, cancellationToken);
    }

    private async Task<VisaoTorneio> ComporAsync(TorneioEntity torneio, Dupla[] times, Partida[] jogos, CancellationToken cancellationToken)
    {
        var ordenados = times.OrderBy(time => time.Seed ?? int.MaxValue).ToArray();
        var jogosOrdenados = jogos.OrderBy(jogo => jogo.Numero).ToArray();
        ProgressaoChave.Sincronizar(jogosOrdenados, ordenados);

        var ids = ordenados.SelectMany(time => new[] { time.JogadorAId, time.JogadorBId }).Distinct().ToArray();
        var elenco = await jogadores.ListarPorIdsAsync(ids, cancellationToken);
        var jogadoresOrdenados = ordenados
            .SelectMany(time => new[]
            {
                elenco.First(jogador => jogador.Id == time.JogadorAId),
                elenco.First(jogador => jogador.Id == time.JogadorBId)
            })
            .ToArray();

        var classificacao = ProgressaoChave.Classificar(jogosOrdenados, ordenados);
        var caminhos = ordenados.ToDictionary(time => time.Id, time => ProgressaoChave.Caminho(time.Id, jogosOrdenados, ordenados));
        return new VisaoTorneio(torneio, jogadoresOrdenados, ordenados, jogosOrdenados, classificacao, ProgressaoChave.CampeaoId(jogosOrdenados, ordenados), caminhos);
    }
}
