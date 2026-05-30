using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Repositories;

/// <summary>
/// Domain repository contract for <see cref="ProductConfiguration"/> aggregate roots.
/// Extends <see cref="IDomainRepository{TEntity}"/> with a query specialised for pricing lookups.
/// </summary>
public interface IProductConfigurationDomainRepository : IDomainRepository<ProductConfiguration>
{
    /// <summary>
    /// Returns the active configuration for the given product code on the specified date, or <c>null</c> if none exists.
    /// </summary>
    /// <param name="productCode">The product identifier to look up.</param>
    /// <param name="date">The date the configuration must be valid on.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching <see cref="ProductConfiguration"/>, or <c>null</c>.</returns>
    Task<ProductConfiguration?> FindByProductCodeAndDate(
        string productCode,
        DateTime date,
        CancellationToken cancellationToken = default);
}
