using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Domain.Pricing.Interfaces;

/// <summary>
/// Computes the available installment payment plans for a given quote result.
/// Returns exactly three plans — one for each supported installment count (1, 2, 4).
/// </summary>
public interface IInstallmentCalculator
{
    /// <summary>
    /// Calculates all installment options from the supplied quote totals.
    /// Surcharge rates for multi-installment plans are applied by the implementation.
    /// </summary>
    /// <param name="quoteResult">The quote result containing net premium, tax, and fee amounts.</param>
    /// <returns>
    /// A read-only list of three <see cref="InstallmentPlan"/> instances ordered by installment count (1, 2, 4).
    /// </returns>
    IReadOnlyList<InstallmentPlan> Calculate(QuoteResult quoteResult);
}
