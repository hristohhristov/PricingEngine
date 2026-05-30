using FluentAssertions;
using PricingEngine.Domain.Pricing.Exceptions;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Tests.Unit.Domain.Pricing;

public class InstallmentPlanTests
{
    private static Money Eur(decimal amount) => new(amount, "EUR");

    // ── Valid counts ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidCount1_CreatesInstance()
    {
        var plan = new InstallmentPlan(1, Eur(100m), Eur(100m), Eur(100m));
        plan.InstallmentCount.Should().Be(1);
    }

    [Fact]
    public void Constructor_ValidCount2_CreatesInstance()
    {
        var plan = new InstallmentPlan(2, Eur(51m), Eur(102m), Eur(100m));
        plan.InstallmentCount.Should().Be(2);
    }

    [Fact]
    public void Constructor_ValidCount4_CreatesInstance()
    {
        var plan = new InstallmentPlan(4, Eur(26.25m), Eur(105m), Eur(100m));
        plan.InstallmentCount.Should().Be(4);
    }

    // ── Invalid counts ───────────────────────────────────────────────────────

    [Fact]
    public void Constructor_InvalidCount3_ThrowsInvalidInstallmentPlanException()
    {
        var act = () => new InstallmentPlan(3, Eur(33.33m), Eur(100m), Eur(100m));
        act.Should().Throw<InvalidInstallmentPlanException>();
    }

    [Fact]
    public void Constructor_InvalidCount0_Throws()
    {
        var act = () => new InstallmentPlan(0, Eur(100m), Eur(100m), Eur(100m));
        act.Should().Throw<InvalidInstallmentPlanException>();
    }

    [Fact]
    public void Constructor_InvalidCount5_Throws()
    {
        var act = () => new InstallmentPlan(5, Eur(20m), Eur(100m), Eur(100m));
        act.Should().Throw<InvalidInstallmentPlanException>();
    }

    // ── Surcharge ────────────────────────────────────────────────────────────

    [Fact]
    public void SurchargeAmount_IsCalculatedAsTotalMinusNetPayable()
    {
        // total = 102, netPayable = 100 → surcharge = 2
        var plan = new InstallmentPlan(2, Eur(51m), Eur(102m), Eur(100m));
        plan.InstallmentSurchargeAmount.Amount.Should().Be(2m);
    }

    [Fact]
    public void SurchargeAmount_WithEqualTotalAndNetPayable_IsZero()
    {
        var plan = new InstallmentPlan(1, Eur(100m), Eur(100m), Eur(100m));
        plan.InstallmentSurchargeAmount.Amount.Should().Be(0m);
    }

    // ── Exposed properties ───────────────────────────────────────────────────

    [Fact]
    public void Properties_ExposedCorrectly()
    {
        var perInstallment = Eur(51m);
        var total          = Eur(102m);
        var net            = Eur(100m);

        var plan = new InstallmentPlan(2, perInstallment, total, net);

        plan.AmountPerInstallment.Should().Be(perInstallment);
        plan.TotalAmount.Should().Be(total);
    }
}
