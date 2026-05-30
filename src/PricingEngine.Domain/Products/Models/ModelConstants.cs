namespace PricingEngine.Domain.Products.Models;

/// <summary>Compile-time constants shared across Products domain models.</summary>
public static class ModelConstants
{
    /// <summary>Constraints governing the <see cref="ProductConfiguration"/> aggregate root.</summary>
    public static class ProductConfiguration
    {
        /// <summary>Minimum number of characters allowed in a product code.</summary>
        public const int MinProductCodeLength = 3;

        /// <summary>Maximum number of characters allowed in a product code.</summary>
        public const int MaxProductCodeLength = 50;

        /// <summary>The earliest date allowed for a configuration's ValidFrom field.</summary>
        public static readonly DateTime MinValidFrom = new(2000, 1, 1);

        /// <summary>The latest date allowed for a configuration's ValidFrom field.</summary>
        public static readonly DateTime MaxValidFrom = new(2100, 1, 1);

        /// <summary>The earliest date allowed for a configuration's ValidTo field.</summary>
        public static readonly DateTime MinValidTo   = new(2000, 1, 1);

        /// <summary>The latest date allowed for a configuration's ValidTo field.</summary>
        public static readonly DateTime MaxValidTo   = new(2100, 1, 1);
    }
}
