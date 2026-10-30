using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Infra.Data.Context;
using Copasogo.Infra.Data.Generic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Copasogo.Infra.Data.Duplas.Repositories;

public sealed class DuplaRepository(AppDbContext context, ICurrentUserContext currentUserContext)
    : Repository<Dupla>(context, currentUserContext), IDuplaRepository
{
    public async Task<IReadOnlyList<Dupla>> ListarPorTorneioAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        return await Context.Duplas.Where(dupla => dupla.TorneioId == torneioId).OrderBy(dupla => dupla.Seed).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Dupla>> ListarPorJogadorAsync(Guid jogadorId, CancellationToken cancellationToken = default)
    {
        return await Context.Duplas
            .Where(dupla => dupla.JogadorAId == jogadorId || dupla.JogadorBId == jogadorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Dupla>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Duplas.ToListAsync(cancellationToken);
    }

    public Task<int> ContarAsync(Guid torneioId, CancellationToken cancellationToken = default)
    {
        return Context.Duplas.CountAsync(dupla => dupla.TorneioId == torneioId, cancellationToken);
    }
}
