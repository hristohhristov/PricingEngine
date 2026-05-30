namespace PricingEngine.Domain.Common;

/// <summary>
/// Contract for providing seed data objects used during database initialisation.
/// Implementations supply typed records for a specific entity type.
/// </summary>
public interface IInitialData
{
    /// <summary>Gets the CLR type of the entity whose seed data this object provides.</summary>
    Type EntityType { get; }

    /// <summary>Returns the collection of seed records to be inserted.</summary>
    /// <returns>An enumerable of seed objects compatible with <see cref="EntityType"/>.</returns>
    IEnumerable<object> GetData();
}
