using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Repositories;

public interface IProductConfigurationDomainRepository : IDomainRepository<ProductConfiguration>
{
    Task<ProductConfiguration?> FindByProductCodeAndDate(
        string productCode,
        DateTime date,
        CancellationToken cancellationToken = default);
}
