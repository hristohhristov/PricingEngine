using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Responses;

/// <summary>
/// Top-level API response returned when a quote is successfully calculated.
/// Includes the quote identifier, product code, financial breakdown, and available installment options.
/// </summary>
/// <param name="QuoteId">The unique identifier assigned to the persisted quote record.</param>
/// <param name="ProductCode">The product code for which the quote was calculated.</param>
/// <param name="Breakdown">The financial breakdown (net premium, tax, fee, total).</param>
/// <param name="InstallmentOptions">The available installment payment plans.</param>
public record QuoteSummaryResponse(
    Guid                                  QuoteId,
    string                                ProductCode,
    QuoteBreakdownResponse                Breakdown,
    IReadOnlyList<InstallmentPlanResponse> InstallmentOptions)
{
    /// <summary>
    /// Constructs a <see cref="QuoteSummaryResponse"/> from domain objects.
    /// </summary>
    /// <param name="quoteId">The identifier of the persisted quote record.</param>
    /// <param name="productCode">The product code used for pricing.</param>
    /// <param name="quoteResult">The domain quote result containing net premium, tax, and fee.</param>
    /// <param name="installmentPlans">The list of installment plans produced by the calculator.</param>
    /// <returns>A fully populated <see cref="QuoteSummaryResponse"/>.</returns>
    public static QuoteSummaryResponse From(
        Guid quoteId,
        string productCode,
        QuoteResult quoteResult,
        IReadOnlyList<InstallmentPlan> installmentPlans)
        => new(
            QuoteId: quoteId,
            ProductCode: productCode,
            Breakdown: new(
                quoteResult.NetPremium.Amount,
                quoteResult.TaxAmount.Amount,
                quoteResult.FeeAmount.Amount,
                quoteResult.TotalAmount.Amount,
                quoteResult.NetPremium.Currency),
            InstallmentOptions: installmentPlans
                .Select(p => new InstallmentPlanResponse(
                    p.InstallmentCount,
                    p.AmountPerInstallment.Amount,
                    p.InstallmentSurchargeAmount.Amount,
                    p.TotalAmount.Amount,
                    p.AmountPerInstallment.Currency))
                .ToList());
}
