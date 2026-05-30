namespace PricingEngine.Domain.Common.Models;

/// <summary>
/// Defines the domain-event contract for all entities.
/// Allows the persistence layer to access and clear pending events after saving.
/// </summary>
public interface IEntity
{
    /// <summary>Gets the domain events that have been raised but not yet dispatched.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Clears all pending domain events from the collection.</summary>
    void ClearEvents();
}
