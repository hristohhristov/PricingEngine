using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application.Products.Common;

/// <summary>
/// Extends <see cref="IProductPricingStrategy"/> with metadata required to seed the database
/// with a default product configuration at application startup.
/// </summary>
public interface IProductDefinition : IProductPricingStrategy
{
    /// <summary>Gets the default product configuration serialised as a JSON string.</summary>
    string DefaultConfigJson { get; }

    /// <summary>Gets the date from which the seeded default configuration is valid.</summary>
    DateTime DefaultValidFrom { get; }

    /// <summary>Gets the date until which the seeded default configuration is valid.</summary>
    DateTime DefaultValidTo { get; }
}
