namespace PricingEngine.Domain.Common;

public abstract class DomainEvent : IDomainEvent { }

public abstract class DomainEvent<TId> : DomainEvent where TId : notnull
{
    public TId Id { get; private set; } = default!;

    public void SetId(TId id) => Id = id;
}
