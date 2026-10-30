using System.Linq.Expressions;
using Copasogo.Domain.Generic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copasogo.Infra.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var isDeletedProperty = Expression.Property(parameter, nameof(Entity.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeletedProperty), parameter);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureAuditColumns<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at");
        builder.Property(entity => entity.CreatedBy).HasColumnName("created_by");
        builder.Property(entity => entity.UpdatedBy).HasColumnName("updated_by");
        builder.Property(entity => entity.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(entity => entity.DeletedAt).HasColumnName("deleted_at");
        builder.Property(entity => entity.DeletedBy).HasColumnName("deleted_by");
    }
}
