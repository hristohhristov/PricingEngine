namespace PricingEngine.Domain.Products.Models;

public static class ModelConstants
{
    public static class ProductConfiguration
    {
        public const int MinProductCodeLength = 3;
        public const int MaxProductCodeLength = 50;

        public static readonly DateTime MinValidFrom = new(2000, 1, 1);
        public static readonly DateTime MaxValidFrom = new(2100, 1, 1);
        public static readonly DateTime MinValidTo   = new(2000, 1, 1);
        public static readonly DateTime MaxValidTo   = new(2100, 1, 1);
    }
}
