using System.Text.Json;

namespace PricingEngine.Web.Features.Quotes.Requests;

public record CalculateQuoteRequest(string ProductCode, JsonElement Payload);
