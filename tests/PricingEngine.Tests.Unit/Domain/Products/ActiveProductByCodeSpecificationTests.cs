using FluentAssertions;
using PricingEngine.Domain.Products.Factories;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Products.Specifications;

namespace PricingEngine.Tests.Unit.Domain.Products;

/// <summary>
/// Tests are run against the compiled Expression (via ToExpression().Compile()) rather than
/// IsSatisfiedBy() to avoid the static ConcurrentDictionary cache in Specification that would
/// cause later tests to reuse the first instance's compiled delegate.
/// </summary>
public class ActiveProductByCodeSpecificationTests
{
    private static ProductConfiguration MakeConfig(
        string   productCode = "HOME_V1",
        DateTime? validFrom  = null,
        DateTime? validTo    = null)
        => new ProductConfigurationFactory()
            .WithProductCode(productCode)
            .WithConfigData("{}")
            .WithValidFrom(validFrom ?? new DateTime(2024, 1, 1))
            .WithValidTo(validTo     ?? new DateTime(2100, 1, 1))
            .Build();

    private static Func<ProductConfiguration, bool> Compile(string productCode, DateTime date)
        => new ActiveProductByCodeSpecification(productCode, date).ToExpression().Compile();

    [Fact]
    public void Matches_ProductCodeAndDateWithinRange_ReturnsTrue()
    {
        var fn     = Compile("HOME_V1", new DateTime(2025, 6, 15));
        var config = MakeConfig("HOME_V1", new DateTime(2024, 1, 1), new DateTime(2100, 1, 1));

        fn(config).Should().BeTrue();
    }

    [Fact]
    public void Matches_WrongProductCode_ReturnsFalse()
    {
        var fn     = Compile("AUTO_V1", new DateTime(2025, 6, 15));
        var config = MakeConfig("HOME_V1");

        fn(config).Should().BeFalse();
    }

    [Fact]
    public void Matches_DateBeforeValidFrom_ReturnsFalse()
    {
        var fn     = Compile("HOME_V1", new DateTime(2023, 12, 31));
        var config = MakeConfig("HOME_V1", new DateTime(2024, 1, 1), new DateTime(2100, 1, 1));

        fn(config).Should().BeFalse();
    }

    [Fact]
    public void Matches_DateAfterValidTo_ReturnsFalse()
    {
        var fn     = Compile("HOME_V1", new DateTime(2025, 1, 1));
        var config = MakeConfig("HOME_V1", new DateTime(2023, 1, 1), new DateTime(2024, 12, 31));

        fn(config).Should().BeFalse();
    }

    [Fact]
    public void Matches_ValidFromEqualsDate_ReturnsTrue()
    {
        var boundary = new DateTime(2024, 1, 1);
        var fn       = Compile("HOME_V1", boundary);
        var config   = MakeConfig("HOME_V1", validFrom: boundary, validTo: new DateTime(2100, 1, 1));

        fn(config).Should().BeTrue();
    }

    [Fact]
    public void Matches_ValidToEqualsDate_ReturnsTrue()
    {
        var boundary = new DateTime(2024, 12, 31);
        var fn       = Compile("HOME_V1", boundary);
        var config   = MakeConfig("HOME_V1", validFrom: new DateTime(2024, 1, 1), validTo: boundary);

        fn(config).Should().BeTrue();
    }

    [Fact]
    public void ToExpression_ReturnsExpression_ThatCompiles()
    {
        var spec = new ActiveProductByCodeSpecification("HOME_V1", new DateTime(2025, 1, 1));
        var compiled = spec.ToExpression().Compile();
        var config   = MakeConfig("HOME_V1");

        // Just assert it doesn't throw and returns a bool
        var result = compiled(config);
        result.Should().BeTrue();
    }
}
