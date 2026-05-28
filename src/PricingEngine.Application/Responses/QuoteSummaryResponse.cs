using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Responses;

public record QuoteSummaryResponse(
    Guid                                  QuoteId,
    string                                ProductCode,
    QuoteBreakdownResponse                Breakdown,
    IReadOnlyList<InstallmentPlanResponse> InstallmentOptions)
{
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
