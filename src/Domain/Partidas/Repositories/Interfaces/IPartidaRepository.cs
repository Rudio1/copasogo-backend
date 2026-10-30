using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Partidas.Entities;

namespace Copasogo.Domain.Partidas.Repositories.Interfaces;

public interface IPartidaRepository : IRepository<Partida>
{
    Task AddRangeAsync(IEnumerable<Partida> partidas, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Partida>> ListarPorTorneioAsync(Guid torneioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Partida>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<Partida?> ObterPorNumeroAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
}
