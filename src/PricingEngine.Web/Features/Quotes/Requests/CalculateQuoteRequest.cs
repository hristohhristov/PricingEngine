using System.Text.Json;

namespace PricingEngine.Web.Features.Quotes.Requests;

/// <summary>
/// HTTP request body for the <c>POST /api/v1/quotes</c> endpoint.
/// </summary>
/// <param name="ProductCode">The identifier of the insurance product to price (e.g., <c>"HOME_V1"</c>).</param>
/// <param name="Payload">A JSON object containing product-specific input parameters (e.g., insured sum, vehicle value).</param>
public record CalculateQuoteRequest(string ProductCode, JsonElement Payload);
