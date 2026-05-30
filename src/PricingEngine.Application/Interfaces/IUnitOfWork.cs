namespace PricingEngine.Application.Interfaces;

/// <summary>
/// Represents a unit of work that batches and atomically commits all pending changes to the data store.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes accumulated within the current scope to the underlying database.
    /// </summary>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all changes have been committed.</returns>
    Task CommitAsync(CancellationToken ct = default);
}
