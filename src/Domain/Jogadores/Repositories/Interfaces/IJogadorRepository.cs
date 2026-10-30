using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Jogadores.Entities;

namespace Copasogo.Domain.Jogadores.Repositories.Interfaces;

public interface IJogadorRepository : IRepository<Jogador>
{
    Task<Jogador?> ObterPorSteamIdAsync(string steamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Jogador>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Jogador>> ListarPorIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
}
