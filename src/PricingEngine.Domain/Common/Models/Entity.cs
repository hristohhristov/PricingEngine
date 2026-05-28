using System.Runtime.CompilerServices;

namespace PricingEngine.Domain.Common.Models;

public abstract class Entity : IEntity, ISoftDelete
{
    private readonly ICollection<IDomainEvent> _domainEvents;

    protected Entity() => _domainEvents = new List<IDomainEvent>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void SoftDelete()
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => _domainEvents.ToList().AsReadOnly();

    public void ClearEvents() => _domainEvents.Clear();

    protected void RaiseEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return false;
    }

    public static bool operator ==(Entity? first, Entity? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.Equals(second);
    }

    public static bool operator !=(Entity? first, Entity? second) => !(first == second);

    public override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);
}

public abstract class Entity<TId> : Entity where TId : notnull
{
    private int? _transientHashCode;

    public TId Id { get; private set; } = default!;

    protected void SetId(TId id) => Id = id;

    public bool IsTransient() => Id.Equals(default(TId));

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (IsTransient() || other.IsTransient()) return false;
        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId>? first, Entity<TId>? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.Equals(second);
    }

    public static bool operator !=(Entity<TId>? first, Entity<TId>? second)
        => !(first == second);

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
