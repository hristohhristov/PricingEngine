namespace PricingEngine.Domain.Common.Models;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearEvents();
}
