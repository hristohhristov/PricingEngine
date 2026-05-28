namespace PricingEngine.Application.Exceptions;

public class ProductConfigurationNotFoundException(string productCode)
    : Exception($"No active configuration found for product '{productCode}'.");
