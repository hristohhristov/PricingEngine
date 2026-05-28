using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Products.Repositories;
using PricingEngine.Domain.Products.Specifications;

namespace PricingEngine.Infrastructure.Persistence.Repositories;

public class ProductConfigurationRepository(PricingDbContext context)
    : DataRepository<PricingDbContext, ProductConfiguration>(context),
      IProductConfigurationDomainRepository
{
    public Task<ProductConfiguration?> FindByProductCodeAndDate(
        string productCode, DateTime date, CancellationToken ct = default)
        => Context.ProductConfigurations
            .Where(new ActiveProductByCodeSpecification(productCode, date))
            .FirstOrDefaultAsync(ct);
}
