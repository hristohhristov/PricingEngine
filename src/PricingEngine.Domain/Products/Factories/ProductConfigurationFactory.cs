using PricingEngine.Domain.Products.Exceptions;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Factories;

/// <summary>
/// Concrete fluent factory for constructing <see cref="ProductConfiguration"/> aggregate roots.
/// Tracks which required fields have been set and throws if any are missing when <see cref="Build"/> is called.
/// </summary>
public class ProductConfigurationFactory : IProductConfigurationFactory
{
    private string   _productCode   = default!;
    private string   _configData    = default!;
    private DateTime _validFrom;
    private DateTime _validTo;

    private bool _productCodeSet = false;
    private bool _configDataSet  = false;
    private bool _validFromSet   = false;
    private bool _validToSet     = false;

    /// <inheritdoc/>
    public IProductConfigurationFactory WithProductCode(string productCode)
    {
        _productCode    = productCode;
        _productCodeSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IProductConfigurationFactory WithConfigData(string configData)
    {
        _configData    = configData;
        _configDataSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IProductConfigurationFactory WithValidFrom(DateTime validFrom)
    {
        _validFrom    = validFrom;
        _validFromSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IProductConfigurationFactory WithValidTo(DateTime validTo)
    {
        _validTo    = validTo;
        _validToSet = true;
        return this;
    }

    /// <summary>
    /// Validates all accumulated state and creates a new <see cref="ProductConfiguration"/> instance.
    /// </summary>
    /// <returns>A fully initialised <see cref="ProductConfiguration"/> aggregate root.</returns>
    /// <exception cref="InvalidProductConfigurationException">
    /// Thrown when any of <c>ProductCode</c>, <c>ConfigData</c>, <c>ValidFrom</c>, or <c>ValidTo</c> have not been set.
    /// </exception>
    public ProductConfiguration Build()
    {
        if (!_productCodeSet || !_configDataSet || !_validFromSet || !_validToSet)
            throw new InvalidProductConfigurationException(
                "ProductCode, ConfigData, ValidFrom, and ValidTo are all required.");

        return new ProductConfiguration(_productCode, _configData, _validFrom, _validTo);
    }
}
