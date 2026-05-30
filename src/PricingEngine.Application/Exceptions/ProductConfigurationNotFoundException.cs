namespace PricingEngine.Application.Exceptions;

/// <summary>
/// Thrown by the command handler when no active <c>ProductConfiguration</c> is found for the requested product code.
/// Maps to HTTP 404 via the exception-handling middleware.
/// </summary>
public class ProductConfigurationNotFoundException(string productCode)
    : Exception($"No active configuration found for product '{productCode}'.");
