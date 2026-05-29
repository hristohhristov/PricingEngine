using System.Text.Json;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.Common;

public abstract class ProductDefinitionBase<TConfig> : BasePricingStrategy, IProductDefinition
    where TConfig : class
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    protected abstract TConfig DefaultConfig { get; }

    public string DefaultConfigJson => JsonSerializer.Serialize(DefaultConfig, JsonOptions);
    public virtual DateTime DefaultValidFrom => new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public virtual DateTime DefaultValidTo   => new(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    protected sealed override (Money netPremium, Money fee) CalculateComponents(
        JsonElement parameters, string configurationJson)
    {
        var config = JsonSerializer.Deserialize<TConfig>(configurationJson, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialise config for '{SupportedProductCode}'.");
        return Calculate(parameters, config);
    }

    protected abstract (Money netPremium, Money fee) Calculate(JsonElement parameters, TConfig config);
}
