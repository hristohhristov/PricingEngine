using System.Text.Json;
using MediatR;
using PricingEngine.Application.Responses;

namespace PricingEngine.Application.Commands;

/// <summary>
/// MediatR command that triggers the full quote calculation pipeline.
/// Carries the product code and the raw JSON payload from the API request.
/// </summary>
/// <param name="ProductCode">The identifier of the product to price (e.g., <c>"HOME_V1"</c>).</param>
/// <param name="Payload">The product-specific JSON parameters supplied by the caller.</param>
public record CalculateQuoteCommand(string ProductCode, JsonElement Payload)
    : IRequest<QuoteSummaryResponse>;
