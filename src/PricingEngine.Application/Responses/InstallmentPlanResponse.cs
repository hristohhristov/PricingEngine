namespace PricingEngine.Application.Responses;

public record InstallmentPlanResponse(
    int     NumberOfInstallments,
    decimal AmountPerInstallment,
    decimal Surcharge,
    decimal TotalAmount,
    string  Currency);
