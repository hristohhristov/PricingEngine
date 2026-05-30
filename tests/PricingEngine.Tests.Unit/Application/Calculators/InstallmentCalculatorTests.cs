using FluentAssertions;
using PricingEngine.Application.Calculators;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Tests.Unit.Application.Calculators;

public class InstallmentCalculatorTests
{
    private readonly InstallmentCalculator _calculator = new();

    private static QuoteResult MakeResult(decimal net, decimal tax, decimal fee)
        => new(new Money(net, "EUR"), new Money(tax, "EUR"), new Money(fee, "EUR"));

    // ── Always 3 plans ────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_ReturnsThreePlans()
    {
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        result.Should().HaveCount(3);
    }

    // ── One installment (no surcharge) ────────────────────────────────────────

    [Fact]
    public void Calculate_PlanWithOneInstallment_HasNoSurcharge()
    {
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        var plan1  = result.Single(p => p.InstallmentCount == 1);
        plan1.InstallmentSurchargeAmount.Amount.Should().Be(0m);
    }

    [Fact]
    public void Calculate_OnePlanTotalEqualsOriginalTotal()
    {
        var quoteResult = MakeResult(500m, 50m, 25m);
        var plans       = _calculator.Calculate(quoteResult);
        var plan1       = plans.Single(p => p.InstallmentCount == 1);

        plan1.TotalAmount.Amount.Should().Be(quoteResult.TotalAmount.Amount);
    }

    // ── Two installments (2% surcharge) ──────────────────────────────────────

    [Fact]
    public void Calculate_PlanWithTwoInstallments_HasTwoPercentSurcharge()
    {
        // total = 575, surcharge = 2% × 575 = 11.50
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        var plan2  = result.Single(p => p.InstallmentCount == 2);
        plan2.InstallmentSurchargeAmount.Amount.Should().Be(11.50m);
    }

    [Fact]
    public void Calculate_PlanWithTwoInstallments_TotalIsSumWithSurcharge()
    {
        // total = 575 + 11.50 = 586.50
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        var plan2  = result.Single(p => p.InstallmentCount == 2);
        plan2.TotalAmount.Amount.Should().Be(586.50m);
    }

    // ── Four installments (5% surcharge) ─────────────────────────────────────

    [Fact]
    public void Calculate_PlanWithFourInstallments_HasFivePercentSurcharge()
    {
        // total = 575, surcharge = 5% × 575 = 28.75
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        var plan4  = result.Single(p => p.InstallmentCount == 4);
        plan4.InstallmentSurchargeAmount.Amount.Should().Be(28.75m);
    }

    [Fact]
    public void Calculate_PlanWithFourInstallments_TotalIsSumWithSurcharge()
    {
        // 575 + 28.75 = 603.75
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        var plan4  = result.Single(p => p.InstallmentCount == 4);
        plan4.TotalAmount.Amount.Should().Be(603.75m);
    }

    // ── Per-installment amount is rounded ────────────────────────────────────

    [Fact]
    public void Calculate_AmountsAreRoundedToTwoDecimals()
    {
        // Use an amount that produces a repeating decimal when divided
        // total = 100 + 10 + 5 = 115
        // plan4: surcharge = 5.75, total = 120.75, per installment = 30.19 (rounded)
        var result = _calculator.Calculate(MakeResult(100m, 10m, 5m));
        var plan4  = result.Single(p => p.InstallmentCount == 4);
        var perInstallmentDecimals = decimal.GetBits(plan4.AmountPerInstallment.Amount)[3] >> 16 & 0xFF;
        perInstallmentDecimals.Should().BeLessOrEqualTo(2);
    }

    // ── Plan installment counts ───────────────────────────────────────────────

    [Fact]
    public void Calculate_PlansHaveCounts1_2_4()
    {
        var result = _calculator.Calculate(MakeResult(500m, 50m, 25m));
        result.Select(p => p.InstallmentCount).Should().BeEquivalentTo([1, 2, 4]);
    }
}
