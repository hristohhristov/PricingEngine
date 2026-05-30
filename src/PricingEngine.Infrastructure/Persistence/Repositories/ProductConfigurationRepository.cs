using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Products.Repositories;
using PricingEngine.Domain.Products.Specifications;

namespace PricingEngine.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IProductConfigurationDomainRepository"/>.
/// Uses <see cref="ActiveProductByCodeSpecification"/> for the product lookup query.
/// </summary>
public class ProductConfigurationRepository(PricingDbContext context)
    : DataRepository<PricingDbContext, ProductConfiguration>(context),
      IProductConfigurationDomainRepository
{
    /// <summary>
    /// Returns the active <see cref="ProductConfiguration"/> matching the given product code and date,
    /// or <c>null</c> when none exists.
    /// </summary>
    /// <param name="productCode">The product identifier to filter by.</param>
    /// <param name="date">The date the configuration must be valid on.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The matching <see cref="ProductConfiguration"/>, or <c>null</c>.</returns>
    public Task<ProductConfiguration?> FindByProductCodeAndDate(
        string productCode, DateTime date, CancellationToken ct = default)
        => Context.ProductConfigurations
            .Where(new ActiveProductByCodeSpecification(productCode, date))
            .FirstOrDefaultAsync(ct);
}
