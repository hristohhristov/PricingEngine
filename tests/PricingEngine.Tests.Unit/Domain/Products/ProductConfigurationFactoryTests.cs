using FluentAssertions;
using PricingEngine.Domain.Products.Exceptions;
using PricingEngine.Domain.Products.Factories;

namespace PricingEngine.Tests.Unit.Domain.Products;

public class ProductConfigurationFactoryTests
{
    private static readonly DateTime ValidFrom = new(2024, 1, 1);
    private static readonly DateTime ValidTo   = new(2100, 1, 1);

    // ── Guard: missing required fields ───────────────────────────────────────

    [Fact]
    public void Build_WithoutProductCode_Throws()
    {
        var act = () => new ProductConfigurationFactory()
            .WithConfigData("{}")
            .WithValidFrom(ValidFrom)
            .WithValidTo(ValidTo)
            .Build();

        act.Should().Throw<InvalidProductConfigurationException>();
    }

    [Fact]
    public void Build_WithoutConfigData_Throws()
    {
        var act = () => new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithValidFrom(ValidFrom)
            .WithValidTo(ValidTo)
            .Build();

        act.Should().Throw<InvalidProductConfigurationException>();
    }

    [Fact]
    public void Build_WithoutValidFrom_Throws()
    {
        var act = () => new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithConfigData("{}")
            .WithValidTo(ValidTo)
            .Build();

        act.Should().Throw<InvalidProductConfigurationException>();
    }

    [Fact]
    public void Build_WithoutValidTo_Throws()
    {
        var act = () => new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithConfigData("{}")
            .WithValidFrom(ValidFrom)
            .Build();

        act.Should().Throw<InvalidProductConfigurationException>();
    }

    // ── Happy path ───────────────────────────────────────────────────────────

    [Fact]
    public void Build_AllRequiredSet_ReturnsProductConfiguration()
    {
        var config = new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithConfigData("{}")
            .WithValidFrom(ValidFrom)
            .WithValidTo(ValidTo)
            .Build();

        config.Should().NotBeNull();
        config.ProductCode.Should().Be("HOME_V1");
        config.Id.Should().NotBeEmpty();
    }

    // ── Fluent API ───────────────────────────────────────────────────────────

    [Fact]
    public void WithProductCode_ReturnsSameInstance()
    {
        var factory  = new ProductConfigurationFactory();
        var returned = factory.WithProductCode("HOME_V1");
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithConfigData_ReturnsSameInstance()
    {
        var factory  = new ProductConfigurationFactory();
        var returned = factory.WithConfigData("{}");
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithValidFrom_ReturnsSameInstance()
    {
        var factory  = new ProductConfigurationFactory();
        var returned = factory.WithValidFrom(ValidFrom);
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithValidTo_ReturnsSameInstance()
    {
        var factory  = new ProductConfigurationFactory();
        var returned = factory.WithValidTo(ValidTo);
        returned.Should().BeSameAs(factory);
    }

    // ── Each Build() produces unique Id ──────────────────────────────────────

    [Fact]
    public void Build_CalledTwice_ProducesDistinctIds()
    {
        var factory = new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithConfigData("{}")
            .WithValidFrom(ValidFrom)
            .WithValidTo(ValidTo);

        var c1 = factory.Build();
        var c2 = factory.Build();

        c1.Id.Should().NotBe(c2.Id);
    }
}
