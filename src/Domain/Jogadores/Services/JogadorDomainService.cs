using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Jogadores.Repositories.Interfaces;
using Copasogo.Domain.Jogadores.Services.Interfaces;
using Copasogo.Domain.Partidas;
using Copasogo.Domain.Partidas.Repositories.Interfaces;

namespace Copasogo.Domain.Jogadores.Services;

public sealed class JogadorDomainService(
    IJogadorRepository jogadores,
    IDuplaRepository duplas,
    IPartidaRepository partidas) : IJogadorDomainService
{
    public async Task<Jogador> CriarAsync(string nome, string steamId, CancellationToken cancellationToken = default)
    {
        var normalizado = Jogador.NormalizarSteamId(steamId);
        if (await jogadores.ObterPorSteamIdAsync(normalizado, cancellationToken) is not null)
        {
            throw new BusinessException(DomainMessages.Jogador.SteamIdJaCadastrado);
        }

        var jogador = Jogador.Criar(nome, normalizado);
        await jogadores.AddAsync(jogador, cancellationToken);
        return jogador;
    }

    public async Task<Jogador> AtualizarAsync(Guid id, string nome, string steamId, CancellationToken cancellationToken = default)
    {
        var jogador = await ObterObrigatorioAsync(id, cancellationToken);
        var normalizado = Jogador.NormalizarSteamId(steamId);
        var existente = await jogadores.ObterPorSteamIdAsync(normalizado, cancellationToken);
        if (existente is not null && existente.Id != jogador.Id)
        {
            throw new BusinessException(DomainMessages.Jogador.SteamIdJaCadastrado);
        }

        jogador.Atualizar(nome, normalizado);
        return jogador;
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await ObterObrigatorioAsync(id, cancellationToken);
        if (await JaCompetiuAsync(jogador.Id, cancellationToken))
        {
            throw new BusinessException(DomainMessages.Jogador.JaCompetiu);
        }

        jogadores.Remove(jogador);
    }

    public async Task<IReadOnlyList<JogadorConsulta>> ListarAsync(Guid? torneioId, CancellationToken cancellationToken = default)
    {
        if (torneioId is null)
        {
            var todos = await jogadores.ListarAsync(cancellationToken);
            return todos.OrderBy(jogador => jogador.Nome).Select(jogador => new JogadorConsulta(jogador, null, null, null)).ToArray();
        }

        var times = await duplas.ListarPorTorneioAsync(torneioId.Value, cancellationToken);
        var ids = times.SelectMany(time => new[] { time.JogadorAId, time.JogadorBId }).Distinct().ToArray();
        var elenco = await jogadores.ListarPorIdsAsync(ids, cancellationToken);
        var ordenados = times
            .OrderBy(time => time.Seed ?? int.MaxValue)
            .SelectMany(time => new[]
            {
                elenco.First(jogador => jogador.Id == time.JogadorAId),
                elenco.First(jogador => jogador.Id == time.JogadorBId)
            })
            .ToArray();
        return await ConsultarAsync(ordenados, torneioId, cancellationToken);
    }

    public async Task<JogadorConsulta> ObterAsync(Guid id, Guid? torneioId, CancellationToken cancellationToken = default)
    {
        var jogador = await ObterObrigatorioAsync(id, cancellationToken);
        var consultas = await ConsultarAsync([jogador], torneioId, cancellationToken);
        return consultas[0];
    }

    private async Task<Jogador> ObterObrigatorioAsync(Guid id, CancellationToken cancellationToken)
    {
        return await jogadores.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Jogador.NaoEncontrado);
    }

    private async Task<bool> JaCompetiuAsync(Guid jogadorId, CancellationToken cancellationToken)
    {
        var times = await duplas.ListarPorJogadorAsync(jogadorId, cancellationToken);
        foreach (var torneioId in times.Select(time => time.TorneioId).Distinct())
        {
            var jogos = await partidas.ListarPorTorneioAsync(torneioId, cancellationToken);
            var elenco = await duplas.ListarPorTorneioAsync(torneioId, cancellationToken);
            foreach (var time in times.Where(time => time.TorneioId == torneioId))
            {
                if (ProgressaoChave.DuplaEntrou(time.Id, jogos, elenco))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task<IReadOnlyList<JogadorConsulta>> ConsultarAsync(
        IReadOnlyList<Jogador> lista,
        Guid? torneioId,
        CancellationToken cancellationToken)
    {
        if (torneioId is null)
        {
            return lista.Select(jogador => new JogadorConsulta(jogador, null, null, null)).ToArray();
        }

        var times = await duplas.ListarPorTorneioAsync(torneioId.Value, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(torneioId.Value, cancellationToken);
        ProgressaoChave.Sincronizar(jogos, times);
        var classificacao = ProgressaoChave.Classificar(jogos, times);
        var porId = lista.ToDictionary(jogador => jogador.Id);
        var faltantes = times
            .Where(time => lista.Any(jogador => time.PossuiJogador(jogador.Id)))
            .SelectMany(time => new[] { time.JogadorAId, time.JogadorBId })
            .Where(id => !porId.ContainsKey(id))
            .Distinct()
            .ToArray();
        foreach (var extra in await jogadores.ListarPorIdsAsync(faltantes, cancellationToken))
        {
            porId[extra.Id] = extra;
        }

        return lista.Select(jogador =>
        {
            var time = times.FirstOrDefault(item => item.PossuiJogador(jogador.Id));
            Jogador? parceiro = null;
            if (time is not null)
            {
                var parceiroId = time.JogadorAId == jogador.Id ? time.JogadorBId : time.JogadorAId;
                porId.TryGetValue(parceiroId, out parceiro);
                if (parceiro is null)
                {
                    parceiro = lista.FirstOrDefault(item => item.Id == parceiroId);
                }
            }

            var estado = time is null ? null : classificacao.FirstOrDefault(item => item.DuplaId == time.Id);
            return new JogadorConsulta(jogador, time, parceiro, estado);
        }).ToArray();
    }
}
