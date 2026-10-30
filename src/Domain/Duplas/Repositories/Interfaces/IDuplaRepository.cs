using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Generic.Repositories.Interfaces;

namespace Copasogo.Domain.Duplas.Repositories.Interfaces;

public interface IDuplaRepository : IRepository<Dupla>
{
    Task<IReadOnlyList<Dupla>> ListarPorTorneioAsync(Guid torneioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Dupla>> ListarPorJogadorAsync(Guid jogadorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Dupla>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<int> ContarAsync(Guid torneioId, CancellationToken cancellationToken = default);
}
