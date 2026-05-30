using FluentAssertions;
using PricingEngine.Domain.Products.Exceptions;
using PricingEngine.Domain.Products.Factories;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Tests.Unit.Domain.Products;

public class ProductConfigurationTests
{
    private static readonly DateTime ValidFrom = new(2024, 1, 1);
    private static readonly DateTime ValidTo   = new(2100, 1, 1);

    private static ProductConfiguration Build(
        string?   productCode = "HOME_V1",
        string?   configData  = "{}",
        DateTime? validFrom   = null,
        DateTime? validTo     = null)
        => new ProductConfigurationFactory()
            .WithProductCode(productCode!)
            .WithConfigData(configData!)
            .WithValidFrom(validFrom ?? ValidFrom)
            .WithValidTo(validTo     ?? ValidTo)
            .Build();

    // ── Construction ─────────────────────────────────────────────────────────

    [Fact]
    public void Build_ValidInputs_CreatesInstance()
    {
        var config = Build();
        config.Should().NotBeNull();
        config.Id.Should().NotBeEmpty();
        config.ProductCode.Should().Be("HOME_V1");
    }

    // ── IsValidOn ────────────────────────────────────────────────────────────

    [Fact]
    public void IsValidOn_DateWithinRange_ReturnsTrue()
    {
        var config = Build(validFrom: new DateTime(2024, 1, 1), validTo: new DateTime(2024, 12, 31));
        config.IsValidOn(new DateTime(2024, 6, 15)).Should().BeTrue();
    }

    [Fact]
    public void IsValidOn_DateBeforeRange_ReturnsFalse()
    {
        var config = Build(validFrom: new DateTime(2024, 1, 1), validTo: new DateTime(2024, 12, 31));
        config.IsValidOn(new DateTime(2023, 12, 31)).Should().BeFalse();
    }

    [Fact]
    public void IsValidOn_DateAfterRange_ReturnsFalse()
    {
        var config = Build(validFrom: new DateTime(2024, 1, 1), validTo: new DateTime(2024, 12, 31));
        config.IsValidOn(new DateTime(2025, 1, 1)).Should().BeFalse();
    }

    [Fact]
    public void IsValidOn_BoundaryValidFromDate_ReturnsTrue()
    {
        var config = Build(validFrom: new DateTime(2024, 1, 1), validTo: new DateTime(2024, 12, 31));
        config.IsValidOn(new DateTime(2024, 1, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsValidOn_BoundaryValidToDate_ReturnsTrue()
    {
        var config = Build(validFrom: new DateTime(2024, 1, 1), validTo: new DateTime(2024, 12, 31));
        config.IsValidOn(new DateTime(2024, 12, 31)).Should().BeTrue();
    }

    // ── IsActive ─────────────────────────────────────────────────────────────

    [Fact]
    public void IsActive_ValidToInFuture_ReturnsTrue()
    {
        var config = Build(validTo: new DateTime(2099, 12, 31));
        config.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_ValidToInPast_ReturnsFalse()
    {
        var config = Build(validFrom: new DateTime(2020, 1, 1), validTo: new DateTime(2021, 1, 1));
        config.IsActive.Should().BeFalse();
    }

    // ── UpdateConfigData ──────────────────────────────────────────────────────

    [Fact]
    public void UpdateConfigData_ValidString_UpdatesConfigData()
    {
        var config = Build();
        config.UpdateConfigData("""{"baseTariff":0.006}""");
        config.ConfigData.Should().Contain("0.006");
    }

    [Fact]
    public void UpdateConfigData_EmptyString_Throws()
    {
        var config = Build();
        var act = () => config.UpdateConfigData("");
        act.Should().Throw<InvalidProductConfigurationException>();
    }

    [Fact]
    public void UpdateConfigData_WhiteSpace_Throws()
    {
        var config = Build();
        var act = () => config.UpdateConfigData("   ");
        act.Should().Throw<InvalidProductConfigurationException>();
    }

    // ── UpdateValidityWindow ──────────────────────────────────────────────────

    [Fact]
    public void UpdateValidityWindow_ValidFromAfterValidTo_Throws()
    {
        var config = Build();
        var act = () => config.UpdateValidityWindow(
            new DateTime(2050, 1, 1),
            new DateTime(2024, 1, 1));
        act.Should().Throw<InvalidProductConfigurationException>()
            .WithMessage("*ValidTo*");
    }

    [Fact]
    public void UpdateValidityWindow_ValidRange_UpdatesValues()
    {
        var config = Build();
        var newFrom = new DateTime(2025, 1, 1);
        var newTo   = new DateTime(2080, 1, 1);
        config.UpdateValidityWindow(newFrom, newTo);
        config.ValidFrom.Should().Be(newFrom);
        config.ValidTo.Should().Be(newTo);
    }

    // ── SoftDelete ────────────────────────────────────────────────────────────

    [Fact]
    public void SoftDelete_SetsIsDeletedTrue_AndDeletedAt()
    {
        var config = Build();
        config.SoftDelete();
        config.IsDeleted.Should().BeTrue();
        config.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void SoftDelete_CalledTwice_DoesNotChangeDeletedAt()
    {
        var config = Build();
        config.SoftDelete();
        var firstDeletedAt = config.DeletedAt;

        // Small delay to check the timestamp stays the same
        config.SoftDelete();

        config.DeletedAt.Should().Be(firstDeletedAt);
    }
}
