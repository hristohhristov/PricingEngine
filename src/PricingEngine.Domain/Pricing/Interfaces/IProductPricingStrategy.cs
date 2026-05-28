using System.Text.Json;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Domain.Pricing.Interfaces;

public interface IProductPricingStrategy
{
    string SupportedProductCode { get; }
    QuoteResult Calculate(JsonElement parameters, string configurationJson);
}
