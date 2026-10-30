using Copasogo.Domain.Generic.Repositories.Interfaces;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Domain.Torneio.Repositories.Interfaces;

public interface ITorneioRepository : IRepository<TorneioEntity>
{
    Task<TorneioEntity?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExisteEdicaoAsync(int edicao, Guid? ignorarId = null, CancellationToken cancellationToken = default);
    Task<bool> ExisteSlugAsync(string slug, Guid? ignorarId = null, CancellationToken cancellationToken = default);
    Task<TorneioEntity?> ObterEmAndamentoAsync(CancellationToken cancellationToken = default);
    Task<TorneioEntity?> ObterAtualAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TorneioEntity>> ListarAsync(CancellationToken cancellationToken = default);
}
