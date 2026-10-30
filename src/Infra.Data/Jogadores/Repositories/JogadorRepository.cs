using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Jogadores.Repositories.Interfaces;
using Copasogo.Infra.Data.Context;
using Copasogo.Infra.Data.Generic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Copasogo.Infra.Data.Jogadores.Repositories;

public sealed class JogadorRepository(AppDbContext context, ICurrentUserContext currentUserContext)
    : Repository<Jogador>(context, currentUserContext), IJogadorRepository
{
    public Task<Jogador?> ObterPorSteamIdAsync(string steamId, CancellationToken cancellationToken = default)
    {
        return Context.Jogadores.FirstOrDefaultAsync(jogador => jogador.SteamId == steamId, cancellationToken);
    }

    public async Task<IReadOnlyList<Jogador>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Jogadores.OrderBy(jogador => jogador.Nome).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Jogador>> ListarPorIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await Context.Jogadores.Where(jogador => ids.Contains(jogador.Id)).ToListAsync(cancellationToken);
    }
}
