using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application.Products.Common;

public interface IProductDefinition : IProductPricingStrategy
{
    string DefaultConfigJson { get; }
    DateTime DefaultValidFrom { get; }
    DateTime DefaultValidTo { get; }
}
