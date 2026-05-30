namespace PricingEngine.Application.Responses;

/// <summary>
/// API response record representing one installment payment option for a quote.
/// </summary>
/// <param name="NumberOfInstallments">The number of installments in this plan (1, 2, or 4).</param>
/// <param name="AmountPerInstallment">The amount due for each individual installment.</param>
/// <param name="Surcharge">Extra amount charged for splitting the payment relative to a single-payment plan.</param>
/// <param name="TotalAmount">Total payable amount including any installment surcharge.</param>
/// <param name="Currency">ISO 4217 currency code for all monetary values in this plan.</param>
public record InstallmentPlanResponse(
    int     NumberOfInstallments,
    decimal AmountPerInstallment,
    decimal Surcharge,
    decimal TotalAmount,
    string  Currency);
