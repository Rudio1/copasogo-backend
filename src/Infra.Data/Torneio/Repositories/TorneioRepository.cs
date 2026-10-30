using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Torneio;
using Copasogo.Domain.Torneio.Repositories.Interfaces;
using Copasogo.Infra.Data.Context;
using Copasogo.Infra.Data.Generic.Repositories;
using Microsoft.EntityFrameworkCore;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Infra.Data.Torneio.Repositories;

public sealed class TorneioRepository(AppDbContext context, ICurrentUserContext currentUserContext)
    : Repository<TorneioEntity>(context, currentUserContext), ITorneioRepository
{
    public Task<TorneioEntity?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return Context.Torneios.FirstOrDefaultAsync(torneio => torneio.Slug == slug, cancellationToken);
    }

    public Task<bool> ExisteEdicaoAsync(int edicao, Guid? ignorarId = null, CancellationToken cancellationToken = default)
    {
        return Context.Torneios.AnyAsync(torneio => torneio.Edicao == edicao && torneio.Id != ignorarId, cancellationToken);
    }

    public Task<bool> ExisteSlugAsync(string slug, Guid? ignorarId = null, CancellationToken cancellationToken = default)
    {
        return Context.Torneios.AnyAsync(torneio => torneio.Slug == slug && torneio.Id != ignorarId, cancellationToken);
    }

    public Task<TorneioEntity?> ObterEmAndamentoAsync(CancellationToken cancellationToken = default)
    {
        return Context.Torneios.FirstOrDefaultAsync(torneio => torneio.Status == StatusTorneio.EmAndamento, cancellationToken);
    }

    public async Task<TorneioEntity?> ObterAtualAsync(CancellationToken cancellationToken = default)
    {
        var atual = await ObterEmAndamentoAsync(cancellationToken);
        if (atual is not null)
        {
            return atual;
        }

        return await Context.Torneios
            .Where(torneio => torneio.Status == StatusTorneio.Encerrado)
            .OrderByDescending(torneio => torneio.Edicao)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TorneioEntity>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Torneios.OrderByDescending(torneio => torneio.Edicao).ToListAsync(cancellationToken);
    }
}
