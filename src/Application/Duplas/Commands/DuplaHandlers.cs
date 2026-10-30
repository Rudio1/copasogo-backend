using Copasogo.Application.Duplas.DataTransfer;
using Copasogo.Domain.Duplas.Services.Interfaces;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using FluentValidation;

namespace Copasogo.Application.Duplas.Commands;

public sealed record SalvarDuplaCommand(Guid? Id, Guid TorneioId, string Name, Guid JogadorAId, Guid JogadorBId);

public interface ICriarDuplaHandler
{
    Task<DuplaResponse> HandleAsync(SalvarDuplaCommand command, CancellationToken cancellationToken = default);
}

public sealed class SalvarDuplaValidator : AbstractValidator<SalvarDuplaCommand>
{
    public SalvarDuplaValidator()
    {
        RuleFor(command => command.TorneioId).NotEmpty().When(command => command.Id is null);
        RuleFor(command => command.Id).NotEmpty().When(command => command.Id is not null);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(80);
        RuleFor(command => command.JogadorAId).NotEmpty();
        RuleFor(command => command.JogadorBId).NotEmpty().NotEqual(command => command.JogadorAId);
    }
}

public sealed class CriarDuplaHandler(IDuplaDomainService duplas, IUnitOfWork unitOfWork, IValidator<SalvarDuplaCommand> validator) : ICriarDuplaHandler
{
    public async Task<DuplaResponse> HandleAsync(SalvarDuplaCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var consulta = await duplas.CriarAsync(command.TorneioId, command.Name, command.JogadorAId, command.JogadorBId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return DuplaResponse.From(consulta);
    }
}

public interface IAtualizarDuplaHandler
{
    Task<DuplaResponse> HandleAsync(SalvarDuplaCommand command, CancellationToken cancellationToken = default);
}

public sealed class AtualizarDuplaHandler(IDuplaDomainService duplas, IUnitOfWork unitOfWork, IValidator<SalvarDuplaCommand> validator) : IAtualizarDuplaHandler
{
    public async Task<DuplaResponse> HandleAsync(SalvarDuplaCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var consulta = await duplas.AtualizarAsync(command.Id ?? Guid.Empty, command.Name, command.JogadorAId, command.JogadorBId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return DuplaResponse.From(consulta);
    }
}

public interface IExcluirDuplaHandler
{
    Task HandleAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class ExcluirDuplaHandler(IDuplaDomainService duplas, IUnitOfWork unitOfWork) : IExcluirDuplaHandler
{
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await duplas.ExcluirAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public interface IListarDuplasHandler
{
    Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default);
}

public sealed class ListarDuplasHandler(IDuplaDomainService duplas) : IListarDuplasHandler
{
    public async Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        var lista = await duplas.ListarAsync(torneioId, cancellationToken);
        return lista.Select(DuplaResponse.From).ToArray();
    }
}

public interface IObterDuplaHandler
{
    Task<DuplaResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ISortearChaveHandler
{
    Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default);
}

public interface IResetarChaveHandler
{
    Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default);
}

public sealed class ResetarChaveHandler(IDuplaDomainService duplas, IUnitOfWork unitOfWork) : IResetarChaveHandler
{
    public async Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        var lista = await duplas.ResetarChaveAsync(torneioId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return lista.Select(DuplaResponse.From).ToArray();
    }
}

public sealed class SortearChaveHandler(IDuplaDomainService duplas, IUnitOfWork unitOfWork) : ISortearChaveHandler
{
    public async Task<IReadOnlyList<DuplaResponse>> HandleAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        var lista = await duplas.SortearAsync(torneioId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return lista.Select(DuplaResponse.From).ToArray();
    }
}

public sealed class ObterDuplaHandler(IDuplaDomainService duplas) : IObterDuplaHandler
{
    public async Task<DuplaResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DuplaResponse.From(await duplas.ObterAsync(id, cancellationToken));
    }
}
