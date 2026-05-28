namespace PricingEngine.Application.Responses;

public record QuoteBreakdownResponse(
    decimal NetPremium,
    decimal TaxAmount,
    decimal FeeAmount,
    decimal TotalAmount,
    string  Currency);
