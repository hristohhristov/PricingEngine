using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Products.Exceptions;

namespace PricingEngine.Domain.Products.Models;

using static ModelConstants.ProductConfiguration;

/// <summary>
/// Aggregate root representing the pricing configuration for a single product version.
/// State changes are made exclusively through domain methods to preserve invariants.
/// </summary>
public class ProductConfiguration : Entity<Guid>, IAggregateRoot
{
    /// <summary>Private parameterless constructor required by EF Core.</summary>
    private ProductConfiguration() { }

    /// <summary>
    /// Creates a new product configuration after validating the product code and validity window.
    /// </summary>
    /// <param name="productCode">The product identifier (e.g., <c>"HOME_V1"</c>).</param>
    /// <param name="configData">Serialised JSON containing product-specific pricing parameters.</param>
    /// <param name="validFrom">Inclusive start date of the validity window.</param>
    /// <param name="validTo">Inclusive end date of the validity window.</param>
    /// <exception cref="InvalidProductConfigurationException">Thrown when any domain invariant is violated.</exception>
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

    /// <summary>Gets the product identifier this configuration belongs to.</summary>
    public string ProductCode { get; private set; } = default!;

    /// <summary>Gets the serialised JSON blob containing product-specific pricing parameters.</summary>
    public string ConfigData { get; private set; } = default!;

    /// <summary>Gets the inclusive start date from which this configuration is valid.</summary>
    public DateTime ValidFrom { get; private set; }

    /// <summary>Gets the inclusive end date until which this configuration is valid.</summary>
    public DateTime ValidTo { get; private set; }

    /// <summary>Gets a value indicating whether this configuration is currently active (ValidTo is today or in the future).</summary>
    public bool IsActive => ValidTo >= DateTime.UtcNow.Date;

    /// <summary>
    /// Determines whether this configuration is valid on the specified date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> when <paramref name="date"/> falls within [ValidFrom, ValidTo]; otherwise <c>false</c>.</returns>
    public bool IsValidOn(DateTime date)
        => ValidFrom.Date <= date.Date && date.Date <= ValidTo.Date;

    /// <summary>
    /// Replaces the current configuration JSON with a new value after validation.
    /// </summary>
    /// <param name="configData">The new serialised JSON configuration; must not be empty.</param>
    /// <exception cref="InvalidProductConfigurationException">Thrown when <paramref name="configData"/> is null or whitespace.</exception>
    public void UpdateConfigData(string configData)
    {
        Guard.AgainstEmptyString<InvalidProductConfigurationException>(
            configData, nameof(ConfigData));
        ConfigData = configData;
    }

    /// <summary>
    /// Updates the validity window after verifying that the new range is internally consistent.
    /// </summary>
    /// <param name="validFrom">New inclusive start date.</param>
    /// <param name="validTo">New inclusive end date; must not precede <paramref name="validFrom"/>.</param>
    /// <exception cref="InvalidProductConfigurationException">Thrown when the new window violates domain constraints.</exception>
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
