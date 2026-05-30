namespace PricingEngine.Domain.Quotes.Models;

/// <summary>Compile-time constants shared across Quotes domain models.</summary>
public static class ModelConstants
{
    /// <summary>Constraints governing the <see cref="QuoteRecord"/> aggregate root's product code field.</summary>
    public static class Quote
    {
        /// <summary>Minimum number of characters allowed in a product code.</summary>
        public const int MinProductCodeLength = 3;

        /// <summary>Maximum number of characters allowed in a product code.</summary>
        public const int MaxProductCodeLength = 50;
    }
}
