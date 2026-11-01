using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Partidas.Repositories.Interfaces;
using Copasogo.Domain.Partidas.Services.Interfaces;
using Copasogo.Domain.Torneio.Repositories.Interfaces;

namespace Copasogo.Domain.Partidas.Services;

public sealed class PartidaDomainService(
    IPartidaRepository partidas,
    IDuplaRepository duplas,
    ITorneioRepository torneios) : IPartidaDomainService
{
    public async Task<IReadOnlyList<Partida>> ListarAsync(Guid torneioId, string? rodadaId, string? status, string? lado, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: false, cancellationToken);
        var (jogos, _) = await CarregarAsync(torneioId, cancellationToken);
        return jogos
            .Where(jogo => rodadaId is null || jogo.RodadaId == rodadaId)
            .Where(jogo => status is null || jogo.StatusApi() == status)
            .Where(jogo => lado is null || LadoApi(jogo.Lado) == lado)
            .OrderBy(jogo => jogo.Numero)
            .ToArray();
    }

    public async Task<Partida> ObterAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: false, cancellationToken);
        var (jogos, _) = await CarregarAsync(torneioId, cancellationToken);
        return jogos.FirstOrDefault(jogo => jogo.Numero == numero)
            ?? throw new NotFoundException(DomainMessages.Partida.NaoEncontrada);
    }

    public async Task<Partida> AgendarAsync(Guid torneioId, int numero, DateTime? agendadaEm, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, _) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        jogo.Agendar(agendadaEm);
        return jogo;
    }

    public async Task<Partida> MarcarAoVivoAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, _) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        jogo.MarcarAoVivo();
        return jogo;
    }

    public async Task<Partida> RegistrarResultadoAsync(Guid torneioId, int numero, Guid vencedorDuplaId, int? placarA, int? placarB, bool wo = false, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, times) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        var (a, b) = ProgressaoChave.Participantes(jogo, jogos, times);
        if (a is null || b is null || (vencedorDuplaId != a.Id && vencedorDuplaId != b.Id))
        {
            throw new BusinessException(DomainMessages.Partida.DuplaNaoJoga);
        }

        if (wo)
        {
            placarA = null;
            placarB = null;
        }
        else if (placarA is int valorA && placarB is int valorB)
        {
            ProgressaoChave.ValidarPlacar(valorA, valorB, vencedorDuplaId == a.Id, numero);
        }
        else if (placarA is not null || placarB is not null)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.PlacarInvalido, numero));
        }

        jogo.RegistrarResultado(vencedorDuplaId, placarA, placarB, wo);
        ProgressaoChave.Sincronizar(jogos, times);
        return jogo;
    }

    public async Task<Partida> CorrigirPlacarAsync(Guid torneioId, int numero, int placarA, int placarB, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, times) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        if (jogo.VencedorDuplaId is null)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.PlacarSoEncerrada, numero));
        }

        var (a, b) = ProgressaoChave.Participantes(jogo, jogos, times);
        if (a is null || b is null || (jogo.VencedorDuplaId != a.Id && jogo.VencedorDuplaId != b.Id))
        {
            throw new BusinessException(DomainMessages.Partida.DuplaNaoJoga);
        }

        ProgressaoChave.ValidarPlacar(placarA, placarB, jogo.VencedorDuplaId == a.Id, numero);
        jogo.CorrigirPlacar(placarA, placarB);
        return jogo;
    }

    public async Task<Partida> ConverterEmWoAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, _) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        jogo.ConverterEmWo();
        return jogo;
    }

    public async Task<Partida> CancelarAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, times) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        jogo.Cancelar();
        ProgressaoChave.Sincronizar(jogos, times);
        return jogo;
    }

    public async Task<Partida> ReabrirAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        await GarantirTorneioAsync(torneioId, editavel: true, cancellationToken);
        var (jogos, times) = await CarregarAsync(torneioId, cancellationToken);
        var jogo = ObterJogo(jogos, numero);
        jogo.Reabrir();
        ProgressaoChave.Sincronizar(jogos, times);
        return jogo;
    }

    private async Task GarantirTorneioAsync(Guid torneioId, bool editavel, CancellationToken cancellationToken)
    {
        var torneio = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        if (editavel)
        {
            torneio.GarantirEditavel();
        }
    }

    private async Task<(IReadOnlyList<Partida> Jogos, IReadOnlyList<Duplas.Entities.Dupla> Times)> CarregarAsync(Guid torneioId, CancellationToken cancellationToken)
    {
        var times = await duplas.ListarPorTorneioAsync(torneioId, cancellationToken);
        var jogos = await partidas.ListarPorTorneioAsync(torneioId, cancellationToken);
        ProgressaoChave.Sincronizar(jogos, times);
        return (jogos, times);
    }

    private static Partida ObterJogo(IReadOnlyList<Partida> jogos, int numero)
    {
        if (numero is < 1 or > 31)
        {
            throw new NotFoundException(DomainMessages.Partida.NaoEncontrada);
        }

        return jogos.FirstOrDefault(jogo => jogo.Numero == numero)
            ?? throw new NotFoundException(DomainMessages.Partida.NaoEncontrada);
    }

    private static string LadoApi(LadoChave lado) => lado switch
    {
        LadoChave.Losers => "losers",
        LadoChave.GrandFinal => "grand_final",
        _ => "winners"
    };
}
