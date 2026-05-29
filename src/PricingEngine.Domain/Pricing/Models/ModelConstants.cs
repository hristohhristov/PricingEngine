namespace PricingEngine.Domain.Pricing.Models;

public static class ModelConstants
{
    public static class Money
    {
        public const decimal MinAmount       = 0.00m;
        public const decimal MaxAmount       = 99_999_999.99m;
        public const string  DefaultCurrency = "EUR";
    }

    public static class Quote
    {
        public const int MinProductCodeLength = 3;
        public const int MaxProductCodeLength = 50;
    }
}
