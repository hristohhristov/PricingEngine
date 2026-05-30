namespace PricingEngine.Application.Responses;

/// <summary>
/// API response record containing the financial breakdown of a calculated quote.
/// </summary>
/// <param name="NetPremium">Base premium amount before tax.</param>
/// <param name="TaxAmount">Tax levied on the net premium.</param>
/// <param name="FeeAmount">Flat product administration fee.</param>
/// <param name="TotalAmount">Total payable amount (net premium + tax + fee).</param>
/// <param name="Currency">ISO 4217 currency code for all monetary values.</param>
public record QuoteBreakdownResponse(
    decimal NetPremium,
    decimal TaxAmount,
    decimal FeeAmount,
    decimal TotalAmount,
    string  Currency);
