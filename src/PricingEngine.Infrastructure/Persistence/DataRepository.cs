using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Common;

namespace PricingEngine.Infrastructure.Persistence;

/// <summary>
/// Generic EF Core repository base class that implements <see cref="IDomainRepository{TEntity}"/>.
/// Adds new entities and updates already-tracked ones, relying on <see cref="IUnitOfWork"/> for the actual commit.
/// </summary>
/// <typeparam name="TDbContext">The EF Core <see cref="DbContext"/> type used by this repository.</typeparam>
/// <typeparam name="TEntity">The aggregate root entity type managed by this repository.</typeparam>
public abstract class DataRepository<TDbContext, TEntity>(TDbContext context)
    where TDbContext : DbContext
    where TEntity    : class, IAggregateRoot
{
    /// <summary>Gets the underlying <typeparamref name="TDbContext"/> instance.</summary>
    protected TDbContext Context => context;

    /// <summary>
    /// Stages the entity for insertion if it is detached, or for update if it is already tracked.
    /// Changes are not persisted until the unit of work is committed.
    /// </summary>
    /// <param name="entity">The aggregate root to save.</param>
    /// <param name="ct">Token used to cancel the operation (unused in this synchronous staging step).</param>
    /// <returns>A completed task.</returns>
    public Task Save(TEntity entity, CancellationToken ct = default)
    {
        if (context.Entry(entity).State == EntityState.Detached)
            context.Set<TEntity>().Add(entity);
        else
            context.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }
}
