using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Copasogo.Infra.Data.Generic.Repositories;

public class Repository<TEntity>(AppDbContext context, ICurrentUserContext currentUserContext) : IRepository<TEntity>
    where TEntity : Entity
{
    protected AppDbContext Context { get; } = context;

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Context.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        entity.Delete(currentUserContext.UserId);
        Context.Set<TEntity>().Update(entity);
    }
}
