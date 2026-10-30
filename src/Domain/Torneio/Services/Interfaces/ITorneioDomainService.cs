using Copasogo.Domain.Torneio.Entities;

namespace Copasogo.Domain.Torneio.Services.Interfaces;

public interface ITorneioDomainService
{
    Task<VisaoTorneio> CriarAsync(string nome, string? slug, int edicao, int entryFeeCents, int prizePoolCents, int quantidadePartidas, CancellationToken cancellationToken = default);
    Task<VisaoTorneio> AtualizarAsync(Guid id, string nome, int entryFeeCents, int prizePoolCents, int quantidadePartidas, CancellationToken cancellationToken = default);
    Task<VisaoTorneio> IniciarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VisaoTorneio> EncerrarAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VisaoTorneio> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VisaoTorneio> ObterAtualAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumoTorneio>> ListarAsync(CancellationToken cancellationToken = default);
}
