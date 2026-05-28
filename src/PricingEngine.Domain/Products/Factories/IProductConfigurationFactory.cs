using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Factories;

public interface IProductConfigurationFactory : IFactory<ProductConfiguration>
{
    IProductConfigurationFactory WithProductCode(string productCode);
    IProductConfigurationFactory WithConfigData(string configData);
    IProductConfigurationFactory WithValidFrom(DateTime validFrom);
    IProductConfigurationFactory WithValidTo(DateTime validTo);
}
