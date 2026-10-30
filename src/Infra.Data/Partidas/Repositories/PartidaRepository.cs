using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Domain.Partidas.Repositories.Interfaces;
using Copasogo.Infra.Data.Context;
using Copasogo.Infra.Data.Generic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Copasogo.Infra.Data.Partidas.Repositories;

public sealed class PartidaRepository(AppDbContext context, ICurrentUserContext currentUserContext)
    : Repository<Partida>(context, currentUserContext), IPartidaRepository
{
    public async Task AddRangeAsync(IEnumerable<Partida> partidas, CancellationToken cancellationToken = default)
    {
        await Context.Partidas.AddRangeAsync(partidas, cancellationToken);
    }

    public async Task<IReadOnlyList<Partida>> ListarPorTorneioAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        return await Context.Partidas.Where(partida => partida.TorneioId == torneioId).OrderBy(partida => partida.Numero).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Partida>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Partidas.ToListAsync(cancellationToken);
    }

    public Task<Partida?> ObterPorNumeroAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default)
    {
        return Context.Partidas.FirstOrDefaultAsync(partida => partida.TorneioId == torneioId && partida.Numero == numero, cancellationToken);
    }
}
