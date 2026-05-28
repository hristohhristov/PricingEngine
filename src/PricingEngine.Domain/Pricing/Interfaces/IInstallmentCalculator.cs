using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Domain.Pricing.Interfaces;

public interface IInstallmentCalculator
{
    // Returns exactly three InstallmentPlan instances (counts: 1, 2, 4).
    // Surcharge rates for 2- and 4-installment plans are applied by the implementation.
    IReadOnlyList<InstallmentPlan> Calculate(QuoteResult quoteResult);
}
