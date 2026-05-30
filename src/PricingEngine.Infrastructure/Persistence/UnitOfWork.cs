using PricingEngine.Application.Interfaces;

namespace PricingEngine.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/> that flushes all pending changes to the database.
/// </summary>
public class UnitOfWork(PricingDbContext context) : IUnitOfWork
{
    /// <summary>
    /// Calls <c>SaveChangesAsync</c> on the <see cref="PricingDbContext"/> to commit all tracked changes.
    /// </summary>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all changes have been committed.</returns>
    public Task CommitAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
