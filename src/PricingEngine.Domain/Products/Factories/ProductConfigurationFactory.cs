using PricingEngine.Domain.Products.Exceptions;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Factories;

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

    public IProductConfigurationFactory WithProductCode(string productCode)
    {
        _productCode    = productCode;
        _productCodeSet = true;
        return this;
    }

    public IProductConfigurationFactory WithConfigData(string configData)
    {
        _configData    = configData;
        _configDataSet = true;
        return this;
    }

    public IProductConfigurationFactory WithValidFrom(DateTime validFrom)
    {
        _validFrom    = validFrom;
        _validFromSet = true;
        return this;
    }

    public IProductConfigurationFactory WithValidTo(DateTime validTo)
    {
        _validTo    = validTo;
        _validToSet = true;
        return this;
    }

    public ProductConfiguration Build()
    {
        if (!_productCodeSet || !_configDataSet || !_validFromSet || !_validToSet)
            throw new InvalidProductConfigurationException(
                "ProductCode, ConfigData, ValidFrom, and ValidTo are all required.");

        return new ProductConfiguration(_productCode, _configData, _validFrom, _validTo);
    }
}
