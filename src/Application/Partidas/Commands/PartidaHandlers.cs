using Copasogo.Application.Torneio.DataTransfer;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Partidas.Services.Interfaces;
using Copasogo.Domain.Torneio.Repositories.Interfaces;
using FluentValidation;

namespace Copasogo.Application.Partidas.Commands;

public sealed record FiltroPartidas(Guid TorneioId, string? RoundId, string? Status, string? Side);

public interface IListarPartidasHandler
{
    Task<IReadOnlyList<PartidaResponse>> HandleAsync(FiltroPartidas filtro, CancellationToken cancellationToken = default);
}

public sealed class ListarPartidasHandler(IPartidaDomainService partidas, ITorneioRepository torneios) : IListarPartidasHandler
{
    public async Task<IReadOnlyList<PartidaResponse>> HandleAsync(FiltroPartidas filtro, CancellationToken cancellationToken = default)
    {
        var quantidade = await PartidaResposta.Quantidade(torneios, filtro.TorneioId, cancellationToken);
        var lista = await partidas.ListarAsync(filtro.TorneioId, filtro.RoundId, filtro.Status, filtro.Side, cancellationToken);
        return lista.Select(partida => PartidaResponse.From(partida, quantidade)).ToArray();
    }
}

public interface IObterPartidaHandler
{
    Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
}

public sealed class ObterPartidaHandler(IPartidaDomainService partidas, ITorneioRepository torneios) : IObterPartidaHandler
{
    public async Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        var quantidade = await PartidaResposta.Quantidade(torneios, torneioId, cancellationToken);
        return PartidaResponse.From(await partidas.ObterAsync(torneioId, numero, cancellationToken), quantidade);
    }
}

public sealed record AgendarPartidaCommand(Guid TorneioId, int Numero, DateTime? ScheduledAt);

public interface IAgendarPartidaHandler
{
    Task<PartidaResponse> HandleAsync(AgendarPartidaCommand command, CancellationToken cancellationToken = default);
}

public sealed class AgendarPartidaHandler(IPartidaDomainService partidas, ITorneioRepository torneios, IUnitOfWork unitOfWork) : IAgendarPartidaHandler
{
    public async Task<PartidaResponse> HandleAsync(AgendarPartidaCommand command, CancellationToken cancellationToken = default)
    {
        var quantidade = await PartidaResposta.Quantidade(torneios, command.TorneioId, cancellationToken);
        var partida = await partidas.AgendarAsync(command.TorneioId, command.Numero, command.ScheduledAt, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PartidaResponse.From(partida, quantidade);
    }
}

public interface IMarcarAoVivoHandler
{
    Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
}

public sealed class MarcarAoVivoHandler(IPartidaDomainService partidas, ITorneioRepository torneios, IUnitOfWork unitOfWork) : IMarcarAoVivoHandler
{
    public async Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        var quantidade = await PartidaResposta.Quantidade(torneios, torneioId, cancellationToken);
        var partida = await partidas.MarcarAoVivoAsync(torneioId, numero, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PartidaResponse.From(partida, quantidade);
    }
}

public sealed record RegistrarResultadoCommand(Guid TorneioId, int Numero, Guid WinnerTeamId, int? ScoreA, int? ScoreB);

public interface IRegistrarResultadoHandler
{
    Task<PartidaResponse> HandleAsync(RegistrarResultadoCommand command, CancellationToken cancellationToken = default);
}

public sealed class RegistrarResultadoValidator : AbstractValidator<RegistrarResultadoCommand>
{
    public RegistrarResultadoValidator()
    {
        RuleFor(command => command.TorneioId).NotEmpty();
        RuleFor(command => command.Numero).InclusiveBetween(1, 31);
        RuleFor(command => command.WinnerTeamId).NotEmpty();
        RuleFor(command => command.ScoreA).GreaterThanOrEqualTo(0).When(command => command.ScoreA is not null);
        RuleFor(command => command.ScoreB).GreaterThanOrEqualTo(0).When(command => command.ScoreB is not null);
    }
}

public sealed class RegistrarResultadoHandler(
    IPartidaDomainService partidas,
    ITorneioRepository torneios,
    IUnitOfWork unitOfWork,
    IValidator<RegistrarResultadoCommand> validator) : IRegistrarResultadoHandler
{
    public async Task<PartidaResponse> HandleAsync(RegistrarResultadoCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var quantidade = await PartidaResposta.Quantidade(torneios, command.TorneioId, cancellationToken);
        var partida = await partidas.RegistrarResultadoAsync(command.TorneioId, command.Numero, command.WinnerTeamId, command.ScoreA, command.ScoreB, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PartidaResponse.From(partida, quantidade);
    }
}

public interface ICancelarPartidaHandler
{
    Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
}

public sealed class CancelarPartidaHandler(IPartidaDomainService partidas, ITorneioRepository torneios, IUnitOfWork unitOfWork) : ICancelarPartidaHandler
{
    public async Task<PartidaResponse> HandleAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        var quantidade = await PartidaResposta.Quantidade(torneios, torneioId, cancellationToken);
        var partida = await partidas.CancelarAsync(torneioId, numero, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PartidaResponse.From(partida, quantidade);
    }
}

file static class PartidaResposta
{
    public static async Task<int> Quantidade(ITorneioRepository torneios, Guid torneioId, CancellationToken cancellationToken)
    {
        var torneio = await torneios.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException(DomainMessages.Torneio.NaoEncontrado);
        return torneio.QuantidadePartidas is 3 or 7 or 15 or 31 ? torneio.QuantidadePartidas : 31;
    }
}
