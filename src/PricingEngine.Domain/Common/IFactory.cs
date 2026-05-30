namespace PricingEngine.Domain.Common;

/// <summary>
/// Generic factory contract for constructing aggregate root instances via a fluent builder API.
/// Implementing classes accumulate configuration through chained <c>With*</c> methods and materialise the entity via <see cref="Build"/>.
/// </summary>
/// <typeparam name="TEntity">The aggregate root type produced by this factory.</typeparam>
public interface IFactory<out TEntity> where TEntity : IAggregateRoot
{
    /// <summary>
    /// Validates all accumulated state and constructs the aggregate root instance.
    /// </summary>
    /// <returns>A fully initialised aggregate root of type <typeparamref name="TEntity"/>.</returns>
    /// <exception cref="BaseDomainException">Thrown when required fields have not been set.</exception>
    TEntity Build();
}
