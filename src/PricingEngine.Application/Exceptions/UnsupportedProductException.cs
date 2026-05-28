namespace PricingEngine.Application.Exceptions;

public class UnsupportedProductException(string productCode)
    : Exception($"No pricing strategy is registered for product '{productCode}'.");
