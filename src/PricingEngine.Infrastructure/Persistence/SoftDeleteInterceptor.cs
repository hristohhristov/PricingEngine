using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Infrastructure.Persistence;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
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
