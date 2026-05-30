namespace PricingEngine.Domain.Common;

/// <summary>
/// Abstract base class for all domain events.
/// Implements <see cref="IDomainEvent"/> and serves as the common ancestor for typed domain events.
/// </summary>
public abstract class DomainEvent : IDomainEvent { }

/// <summary>
/// Abstract base class for domain events that carry a strongly-typed identifier.
/// Exposes <see cref="Id"/> which must be set via <see cref="SetId"/> before the event is dispatched.
/// </summary>
/// <typeparam name="TId">The type of the event identifier; must be non-nullable.</typeparam>
public abstract class DomainEvent<TId> : DomainEvent where TId : notnull
{
    /// <summary>Gets the unique identifier of this domain event instance.</summary>
    public TId Id { get; private set; } = default!;

    /// <summary>Sets the unique identifier for this domain event.</summary>
    /// <param name="id">The identifier value to assign.</param>
    public void SetId(TId id) => Id = id;
}
