using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Factories;

/// <summary>
/// Fluent factory contract for building <see cref="ProductConfiguration"/> aggregate roots.
/// All <c>With*</c> methods return the same factory instance to support method chaining.
/// </summary>
public interface IProductConfigurationFactory : IFactory<ProductConfiguration>
{
    /// <summary>Sets the product code for the configuration being built.</summary>
    /// <param name="productCode">The product identifier (e.g., <c>"HOME_V1"</c>).</param>
    /// <returns>The same factory instance for chaining.</returns>
    IProductConfigurationFactory WithProductCode(string productCode);

    /// <summary>Sets the JSON configuration data for the product.</summary>
    /// <param name="configData">Serialised JSON containing product-specific pricing parameters.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IProductConfigurationFactory WithConfigData(string configData);

    /// <summary>Sets the date from which this configuration becomes active.</summary>
    /// <param name="validFrom">Inclusive start date of the validity window.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IProductConfigurationFactory WithValidFrom(DateTime validFrom);

    /// <summary>Sets the date after which this configuration is no longer active.</summary>
    /// <param name="validTo">Inclusive end date of the validity window.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IProductConfigurationFactory WithValidTo(DateTime validTo);
}
