using Copasogo.Application.Jogadores.DataTransfer;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Jogadores.Services.Interfaces;
using FluentValidation;

namespace Copasogo.Application.Jogadores.Commands;

public sealed record CriarJogadorCommand(string Name, string SteamId);

public interface ICriarJogadorHandler
{
    Task<JogadorResponse> HandleAsync(CriarJogadorCommand command, CancellationToken cancellationToken = default);
}

public sealed class CriarJogadorValidator : AbstractValidator<CriarJogadorCommand>
{
    public CriarJogadorValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(80);
        RuleFor(command => command.SteamId).NotEmpty().MaximumLength(200).WithMessage("O Steam ID aceita ate 200 caracteres.");
    }
}

public sealed class CriarJogadorHandler(IJogadorDomainService jogadores, IUnitOfWork unitOfWork, IValidator<CriarJogadorCommand> validator) : ICriarJogadorHandler
{
    public async Task<JogadorResponse> HandleAsync(CriarJogadorCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var jogador = await jogadores.CriarAsync(command.Name, command.SteamId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return JogadorResponse.From(new JogadorConsulta(jogador, null, null, null));
    }
}

public sealed record AtualizarJogadorCommand(Guid Id, string Name, string SteamId);

public interface IAtualizarJogadorHandler
{
    Task<JogadorResponse> HandleAsync(AtualizarJogadorCommand command, CancellationToken cancellationToken = default);
}

public sealed class AtualizarJogadorValidator : AbstractValidator<AtualizarJogadorCommand>
{
    public AtualizarJogadorValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(80);
        RuleFor(command => command.SteamId).NotEmpty().MaximumLength(200).WithMessage("O Steam ID aceita ate 200 caracteres.");
    }
}

public sealed class AtualizarJogadorHandler(IJogadorDomainService jogadores, IUnitOfWork unitOfWork, IValidator<AtualizarJogadorCommand> validator) : IAtualizarJogadorHandler
{
    public async Task<JogadorResponse> HandleAsync(AtualizarJogadorCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var jogador = await jogadores.AtualizarAsync(command.Id, command.Name, command.SteamId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return JogadorResponse.From(new JogadorConsulta(jogador, null, null, null));
    }
}

public interface IExcluirJogadorHandler
{
    Task HandleAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class ExcluirJogadorHandler(IJogadorDomainService jogadores, IUnitOfWork unitOfWork) : IExcluirJogadorHandler
{
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await jogadores.ExcluirAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public interface IListarJogadoresHandler
{
    Task<IReadOnlyList<JogadorResponse>> HandleAsync(Guid? torneioId, CancellationToken cancellationToken = default);
}

public sealed class ListarJogadoresHandler(IJogadorDomainService jogadores) : IListarJogadoresHandler
{
    public async Task<IReadOnlyList<JogadorResponse>> HandleAsync(Guid? torneioId, CancellationToken cancellationToken = default)
    {
        var lista = await jogadores.ListarAsync(torneioId, cancellationToken);
        return lista.Select(JogadorResponse.From).ToArray();
    }
}

public interface IObterJogadorHandler
{
    Task<JogadorResponse> HandleAsync(Guid id, Guid? torneioId, CancellationToken cancellationToken = default);
}

public sealed class ObterJogadorHandler(IJogadorDomainService jogadores) : IObterJogadorHandler
{
    public async Task<JogadorResponse> HandleAsync(Guid id, Guid? torneioId, CancellationToken cancellationToken = default)
    {
        return JogadorResponse.From(await jogadores.ObterAsync(id, torneioId, cancellationToken));
    }
}
