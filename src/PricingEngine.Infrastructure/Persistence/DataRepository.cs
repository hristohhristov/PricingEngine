using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Common;

namespace PricingEngine.Infrastructure.Persistence;

public abstract class DataRepository<TDbContext, TEntity>(TDbContext context)
    where TDbContext : DbContext
    where TEntity    : class, IAggregateRoot
{
    protected TDbContext Context => context;

    public Task Save(TEntity entity, CancellationToken ct = default)
    {
        if (context.Entry(entity).State == EntityState.Detached)
            context.Set<TEntity>().Add(entity);
        else
            context.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }
}
