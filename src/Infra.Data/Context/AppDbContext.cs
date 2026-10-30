using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Infra.Data.Auth.Identity;
using Copasogo.Infra.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Infra.Data.Context;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ICurrentUserContext currentUserContext) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IUnitOfWork
{
    public DbSet<TorneioEntity> Torneios => Set<TorneioEntity>();
    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<Dupla> Duplas => Set<Dupla>();
    public DbSet<Partida> Partidas => Set<Partida>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ApplySoftDeleteFilters();
    }

    private void ApplyAuditInfo()
    {
        var userId = currentUserContext.UserId;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedAudit(userId);
                    entry.Entity.SetUpdatedAudit(userId);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdatedAudit(userId);
                    break;
            }
        }
    }
}
