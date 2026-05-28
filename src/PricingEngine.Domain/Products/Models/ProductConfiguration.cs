using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Products.Exceptions;

namespace PricingEngine.Domain.Products.Models;

using static ModelConstants.ProductConfiguration;

public class ProductConfiguration : Entity<Guid>, IAggregateRoot
{
    private ProductConfiguration() { }

    internal ProductConfiguration(
        string productCode,
        string configData,
        DateTime validFrom,
        DateTime validTo)
    {
        Validate(productCode, validFrom, validTo);
        ProductCode = productCode;
        ConfigData = configData;
        ValidFrom = validFrom;
        ValidTo = validTo;
        SetId(Guid.NewGuid());
    }

    public string ProductCode { get; private set; } = default!;

    public string ConfigData { get; private set; } = default!;

    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }

    public bool IsActive => ValidTo >= DateTime.UtcNow.Date;

    public bool IsValidOn(DateTime date)
        => ValidFrom.Date <= date.Date && date.Date <= ValidTo.Date;

    public void UpdateConfigData(string configData)
    {
        Guard.AgainstEmptyString<InvalidProductConfigurationException>(
            configData, nameof(ConfigData));
        ConfigData = configData;
    }

    public void UpdateValidityWindow(DateTime validFrom, DateTime validTo)
    {
        ValidateWindow(validFrom, validTo);
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    private static void Validate(string productCode, DateTime validFrom, DateTime validTo)
    {
        Guard.ForStringLength<InvalidProductConfigurationException>(
            productCode, MinProductCodeLength, MaxProductCodeLength, nameof(ProductCode));
        ValidateWindow(validFrom, validTo);
    }

    private static void ValidateWindow(DateTime validFrom, DateTime validTo)
    {
        Guard.AgainstOutOfRange<InvalidProductConfigurationException>(
            validFrom, MinValidFrom, MaxValidFrom, nameof(ValidFrom));
        Guard.AgainstOutOfRange<InvalidProductConfigurationException>(
            validTo, MinValidTo, MaxValidTo, nameof(ValidTo));

        if (validTo < validFrom)
            throw new InvalidProductConfigurationException(
                "ValidTo must be greater than or equal to ValidFrom.");
    }
}
