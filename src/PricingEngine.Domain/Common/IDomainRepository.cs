namespace PricingEngine.Domain.Common;

/// <summary>
/// Generic repository contract for persisting aggregate roots.
/// Implementations handle the actual storage mechanism (e.g., EF Core).
/// </summary>
/// <typeparam name="TEntity">The aggregate root type managed by this repository.</typeparam>
public interface IDomainRepository<in TEntity> where TEntity : IAggregateRoot
{
    /// <summary>
    /// Persists the given entity — adds it if new, updates it if already tracked.
    /// </summary>
    /// <param name="entity">The aggregate root instance to save.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the entity has been staged for persistence.</returns>
    Task Save(TEntity entity, CancellationToken cancellationToken = default);
}
