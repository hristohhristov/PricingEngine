using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Infrastructure.Persistence;

/// <summary>
/// EF Core save-changes interceptor that converts hard deletes into soft deletes for <see cref="Entity"/>-derived types.
/// When an entity is in the <see cref="EntityState.Deleted"/> state, the interceptor switches it to
/// <see cref="EntityState.Modified"/> and calls <see cref="Entity.SoftDelete"/>.
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the synchronous save-changes call and converts deletions to soft deletes.
    /// </summary>
    /// <param name="eventData">Context event data, including the <see cref="DbContext"/> being saved.</param>
    /// <param name="result">The interception result from upstream interceptors.</param>
    /// <returns>The (possibly modified) interception result.</returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null) return result;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: Entity entity })
            {
                entry.State = EntityState.Modified;
                entity.SoftDelete();
            }
        }

        return result;
    }
}
