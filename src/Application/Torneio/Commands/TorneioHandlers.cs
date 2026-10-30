using Copasogo.Application.Torneio.DataTransfer;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Torneio.Services.Interfaces;
using FluentValidation;

namespace Copasogo.Application.Torneio.Commands;

public sealed record CriarTorneioCommand(string Name, string? Slug, int Edicao, int EntryFeeCents, int PrizePoolCents, int MatchCount);

public interface ICriarTorneioHandler
{
    Task<TorneioResponse> HandleAsync(CriarTorneioCommand command, CancellationToken cancellationToken = default);
}

public sealed class CriarTorneioValidator : AbstractValidator<CriarTorneioCommand>
{
    public CriarTorneioValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Edicao).GreaterThan(0);
        RuleFor(command => command.EntryFeeCents).GreaterThanOrEqualTo(0);
        RuleFor(command => command.PrizePoolCents).GreaterThanOrEqualTo(0);
        RuleFor(command => command.MatchCount).Must(quantidade => quantidade is 3 or 7 or 15 or 31).WithMessage("A chave aceita 3, 7, 15 ou 31 partidas.");
        RuleFor(command => command.Slug).MaximumLength(80);
    }
}

public sealed class CriarTorneioHandler(ITorneioDomainService torneios, IUnitOfWork unitOfWork, IValidator<CriarTorneioCommand> validator) : ICriarTorneioHandler
{
    public async Task<TorneioResponse> HandleAsync(CriarTorneioCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var visao = await torneios.CriarAsync(command.Name, command.Slug, command.Edicao, command.EntryFeeCents, command.PrizePoolCents, command.MatchCount, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TorneioResponse.From(visao);
    }
}

public sealed record AtualizarTorneioCommand(Guid Id, string Name, int EntryFeeCents, int PrizePoolCents, int MatchCount);

public interface IAtualizarTorneioHandler
{
    Task<TorneioResponse> HandleAsync(AtualizarTorneioCommand command, CancellationToken cancellationToken = default);
}

public sealed class AtualizarTorneioValidator : AbstractValidator<AtualizarTorneioCommand>
{
    public AtualizarTorneioValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.EntryFeeCents).GreaterThanOrEqualTo(0);
        RuleFor(command => command.PrizePoolCents).GreaterThanOrEqualTo(0);
        RuleFor(command => command.MatchCount).Must(quantidade => quantidade is 3 or 7 or 15 or 31).WithMessage("A chave aceita 3, 7, 15 ou 31 partidas.");
    }
}

public sealed class AtualizarTorneioHandler(ITorneioDomainService torneios, IUnitOfWork unitOfWork, IValidator<AtualizarTorneioCommand> validator) : IAtualizarTorneioHandler
{
    public async Task<TorneioResponse> HandleAsync(AtualizarTorneioCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var visao = await torneios.AtualizarAsync(command.Id, command.Name, command.EntryFeeCents, command.PrizePoolCents, command.MatchCount, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TorneioResponse.From(visao);
    }
}

public sealed record TorneioIdCommand(Guid Id);

public interface IIniciarTorneioHandler
{
    Task<TorneioResponse> HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default);
}

public sealed class IniciarTorneioHandler(ITorneioDomainService torneios, IUnitOfWork unitOfWork) : IIniciarTorneioHandler
{
    public async Task<TorneioResponse> HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default)
    {
        var visao = await torneios.IniciarAsync(command.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TorneioResponse.From(visao);
    }
}

public interface IEncerrarTorneioHandler
{
    Task<TorneioResponse> HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default);
}

public sealed class EncerrarTorneioHandler(ITorneioDomainService torneios, IUnitOfWork unitOfWork) : IEncerrarTorneioHandler
{
    public async Task<TorneioResponse> HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default)
    {
        var visao = await torneios.EncerrarAsync(command.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TorneioResponse.From(visao);
    }
}

public interface IExcluirTorneioHandler
{
    Task HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default);
}

public sealed class ExcluirTorneioHandler(ITorneioDomainService torneios, IUnitOfWork unitOfWork) : IExcluirTorneioHandler
{
    public async Task HandleAsync(TorneioIdCommand command, CancellationToken cancellationToken = default)
    {
        await torneios.ExcluirAsync(command.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public interface IObterTorneioHandler
{
    Task<TorneioResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class ObterTorneioHandler(ITorneioDomainService torneios) : IObterTorneioHandler
{
    public async Task<TorneioResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return TorneioResponse.From(await torneios.ObterAsync(id, cancellationToken));
    }
}

public interface IObterTorneioAtualHandler
{
    Task<TorneioResponse> HandleAsync(CancellationToken cancellationToken = default);
}

public sealed class ObterTorneioAtualHandler(ITorneioDomainService torneios) : IObterTorneioAtualHandler
{
    public async Task<TorneioResponse> HandleAsync(CancellationToken cancellationToken = default)
    {
        return TorneioResponse.From(await torneios.ObterAtualAsync(cancellationToken));
    }
}

public interface IListarTorneiosHandler
{
    Task<IReadOnlyList<TorneioResumoResponse>> HandleAsync(CancellationToken cancellationToken = default);
}

public sealed class ListarTorneiosHandler(ITorneioDomainService torneios) : IListarTorneiosHandler
{
    public async Task<IReadOnlyList<TorneioResumoResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var lista = await torneios.ListarAsync(cancellationToken);
        return lista.Select(TorneioResumoResponse.From).ToArray();
    }
}
