namespace PricingEngine.Domain.Pricing.Models;

/// <summary>Compile-time constants shared across Pricing domain models.</summary>
public static class ModelConstants
{
    /// <summary>Constraints governing valid <see cref="Money"/> values.</summary>
    public static class Money
    {
        /// <summary>The minimum allowed monetary amount (0.00).</summary>
        public const decimal MinAmount       = 0.00m;

        /// <summary>The maximum allowed monetary amount (99,999,999.99).</summary>
        public const decimal MaxAmount       = 99_999_999.99m;

        /// <summary>The ISO 4217 currency code used when none is explicitly specified.</summary>
        public const string  DefaultCurrency = "EUR";
    }

    /// <summary>Constraints governing the product code field on a quote.</summary>
    public static class Quote
    {
        /// <summary>Minimum number of characters allowed in a product code.</summary>
        public const int MinProductCodeLength = 3;

        /// <summary>Maximum number of characters allowed in a product code.</summary>
        public const int MaxProductCodeLength = 50;
    }
}
