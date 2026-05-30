using System.Runtime.CompilerServices;

namespace PricingEngine.Domain.Common.Models;

/// <summary>
/// Abstract base class for all domain entities without a typed identifier.
/// Implements soft-delete semantics and domain event collection.
/// </summary>
public abstract class Entity : IEntity, ISoftDelete
{
    private readonly ICollection<IDomainEvent> _domainEvents;

    /// <summary>Initialises the domain-event collection.</summary>
    protected Entity() => _domainEvents = new List<IDomainEvent>();

    /// <inheritdoc/>
    public bool IsDeleted { get; set; }

    /// <inheritdoc/>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Marks this entity as logically deleted and records the deletion timestamp.
    /// Calling this method more than once is a no-op.
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>Gets the read-only collection of domain events raised by this entity.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => _domainEvents.ToList().AsReadOnly();

    /// <summary>Removes all pending domain events from the collection.</summary>
    public void ClearEvents() => _domainEvents.Clear();

    /// <summary>Appends a domain event to the entity's pending event collection.</summary>
    /// <param name="domainEvent">The event to raise.</param>
    protected void RaiseEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Determines whether the specified object is equal to this entity.
    /// Two entities of different concrete types are never equal.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if both references point to the same instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return false;
    }

    /// <summary>Returns <c>true</c> when both entities are the same reference or both are <c>null</c>.</summary>
    /// <param name="first">Left operand.</param>
    /// <param name="second">Right operand.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(Entity? first, Entity? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.Equals(second);
    }

    /// <summary>Returns <c>true</c> when the two entities are not equal.</summary>
    /// <param name="first">Left operand.</param>
    /// <param name="second">Right operand.</param>
    /// <returns><c>true</c> when not equal; otherwise <c>false</c>.</returns>
    public static bool operator !=(Entity? first, Entity? second) => !(first == second);

    /// <summary>Returns a hash code based on object identity (reference equality).</summary>
    /// <returns>The runtime identity hash code.</returns>
    public override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);
}

/// <summary>
/// Abstract base class for domain entities with a strongly-typed identifier.
/// Provides identity-based equality and a transient hash code for new (unsaved) entities.
/// </summary>
/// <typeparam name="TId">The type of the entity's primary key; must be non-nullable.</typeparam>
public abstract class Entity<TId> : Entity where TId : notnull
{
    private int? _transientHashCode;

    /// <summary>Gets the unique identifier of this entity.</summary>
    public TId Id { get; private set; } = default!;

    /// <summary>Sets the entity's identifier; should be called once during construction.</summary>
    /// <param name="id">The identifier value to assign.</param>
    protected void SetId(TId id) => Id = id;

    /// <summary>
    /// Indicates whether this entity is transient — i.e., its <see cref="Id"/> is the default value
    /// and it has not yet been persisted.
    /// </summary>
    /// <returns><c>true</c> if the entity has no assigned identity; otherwise <c>false</c>.</returns>
    public bool IsTransient() => Id.Equals(default(TId));

    /// <summary>
    /// Determines equality based on type and identifier.
    /// Two transient entities are never considered equal even if they share the same default id.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if both entities have the same type and identifier; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (IsTransient() || other.IsTransient()) return false;
        return Id.Equals(other.Id);
    }

    /// <summary>Returns <c>true</c> when both typed entities are equal.</summary>
    /// <param name="first">Left operand.</param>
    /// <param name="second">Right operand.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(Entity<TId>? first, Entity<TId>? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.Equals(second);
    }

    /// <summary>Returns <c>true</c> when the two typed entities are not equal.</summary>
    /// <param name="first">Left operand.</param>
    /// <param name="second">Right operand.</param>
    /// <returns><c>true</c> when not equal; otherwise <c>false</c>.</returns>
    public static bool operator !=(Entity<TId>? first, Entity<TId>? second)
        => !(first == second);

    /// <summary>
    /// Returns a stable hash code based on the entity's identifier once persisted,
    /// or a transient identity-based hash code for new entities.
    /// </summary>
    /// <returns>A hash code suitable for use in dictionaries and sets.</returns>
    public override int GetHashCode()
    {
        if (IsTransient())
        {
            _transientHashCode ??= RuntimeHelpers.GetHashCode(this);
            return _transientHashCode.Value;
        }
        return Id.GetHashCode() ^ (GetType().GetHashCode() * 31);
    }
}
