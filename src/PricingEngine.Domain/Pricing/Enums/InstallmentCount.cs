namespace PricingEngine.Domain.Pricing.Enums;

/// <summary>
/// Defines the allowed number of premium installments for a quote.
/// Only 1, 2, or 4 installments are supported by the domain rules.
/// </summary>
public enum InstallmentCount
{
    /// <summary>Single lump-sum payment — no surcharge applied.</summary>
    One  = 1,

    /// <summary>Two equal installments — a surcharge is applied to the total.</summary>
    Two  = 2,

    /// <summary>Four equal quarterly installments — the highest surcharge is applied.</summary>
    Four = 4
}
