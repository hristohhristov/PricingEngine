namespace PricingEngine.Application.Exceptions;

/// <summary>
/// Thrown by <c>PricingStrategyFactory</c> when no registered strategy handles the requested product code.
/// Maps to HTTP 422 (Unprocessable Entity) via the exception-handling middleware.
/// </summary>
public class UnsupportedProductException(string productCode)
    : Exception($"No pricing strategy is registered for product '{productCode}'.");
