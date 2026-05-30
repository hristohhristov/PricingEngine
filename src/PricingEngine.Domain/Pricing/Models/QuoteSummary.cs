using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Domain.Pricing.Models;

/// <summary>
/// Value object that aggregates a quote result with its available installment payment plans.
/// Used internally when the full quote summary must be passed between domain services.
/// </summary>
public class QuoteSummary : ValueObject
{
    private readonly QuoteResult                    _quoteResult;
    private readonly IReadOnlyList<InstallmentPlan> _installmentPlans;

    /// <summary>Initialises a new quote summary with the financial result and the set of installment options.</summary>
    /// <param name="quoteResult">The financial breakdown of the quote.</param>
    /// <param name="installmentPlans">The available installment payment plans.</param>
    public QuoteSummary(QuoteResult quoteResult, IReadOnlyList<InstallmentPlan> installmentPlans)
    {
        _quoteResult      = quoteResult;
        _installmentPlans = installmentPlans;
    }

    /// <summary>Gets the financial breakdown of the calculated quote.</summary>
    public QuoteResult                    QuoteResult      => _quoteResult;

    /// <summary>Gets all available installment payment plans.</summary>
    public IReadOnlyList<InstallmentPlan> InstallmentPlans => _installmentPlans;

    /// <summary>
    /// Returns the installment plan for the specified count, or <c>null</c> when no matching plan exists.
    /// </summary>
    /// <param name="installmentCount">The number of installments to look up (1, 2, or 4).</param>
    /// <returns>The matching <see cref="InstallmentPlan"/>, or <c>null</c>.</returns>
    public InstallmentPlan? GetPlan(int installmentCount)
        => _installmentPlans.FirstOrDefault(p => p.InstallmentCount == installmentCount);
}
