using System.Text.Json;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.Common;

/// <summary>
/// Abstract base class that combines <see cref="BasePricingStrategy"/> with <see cref="IProductDefinition"/>.
/// Handles JSON (de)serialisation of the strongly-typed config object and provides sensible defaults for the validity window.
/// </summary>
/// <typeparam name="TConfig">The strongly-typed configuration record for the product.</typeparam>
public abstract class ProductDefinitionBase<TConfig> : BasePricingStrategy, IProductDefinition
    where TConfig : class
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    /// <summary>Gets the default configuration instance used for initial seeding.</summary>
    protected abstract TConfig DefaultConfig { get; }

    /// <summary>Gets the default configuration serialised to JSON; used by the product configuration seeder.</summary>
    public string DefaultConfigJson => JsonSerializer.Serialize(DefaultConfig, JsonOptions);

    /// <summary>Gets the start of the validity window for the seeded default configuration (1 January 2024 UTC).</summary>
    public virtual DateTime DefaultValidFrom => new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>Gets the end of the validity window for the seeded default configuration (1 January 2100 UTC).</summary>
    public virtual DateTime DefaultValidTo   => new(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Deserialises <paramref name="configurationJson"/> into <typeparamref name="TConfig"/> and delegates to the abstract
    /// <see cref="Calculate(JsonElement, TConfig)"/> method.
    /// </summary>
    /// <param name="parameters">The product-specific JSON payload from the API request.</param>
    /// <param name="configurationJson">The active product configuration as a JSON string.</param>
    /// <returns>A tuple containing the net premium and fee amounts.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration JSON cannot be deserialised.</exception>
    protected sealed override (Money netPremium, Money fee) CalculateComponents(
        JsonElement parameters, string configurationJson)
    {
        var config = JsonSerializer.Deserialize<TConfig>(configurationJson, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialise config for '{SupportedProductCode}'.");
        return Calculate(parameters, config);
    }

    /// <summary>
    /// Computes the net premium and flat fee for the given request parameters and typed configuration.
    /// </summary>
    /// <param name="parameters">The product-specific JSON payload from the API request.</param>
    /// <param name="config">The deserialised product configuration.</param>
    /// <returns>A tuple containing the net premium and fee as <see cref="Money"/> values.</returns>
    protected abstract (Money netPremium, Money fee) Calculate(JsonElement parameters, TConfig config);
}
