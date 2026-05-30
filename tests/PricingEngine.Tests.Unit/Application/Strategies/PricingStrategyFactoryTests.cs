using FluentAssertions;
using NSubstitute;
using PricingEngine.Application.Exceptions;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Tests.Unit.Application.Strategies;

public class PricingStrategyFactoryTests
{
    private static IProductPricingStrategy MakeStrategy(string code)
    {
        var strategy = Substitute.For<IProductPricingStrategy>();
        strategy.SupportedProductCode.Returns(code);
        return strategy;
    }

    [Fact]
    public void Resolve_RegisteredCode_ReturnsStrategy()
    {
        var strategy = MakeStrategy("HOME_V1");
        var factory  = new PricingStrategyFactory([strategy]);

        factory.Resolve("HOME_V1").Should().BeSameAs(strategy);
    }

    [Fact]
    public void Resolve_CaseInsensitive_ReturnsStrategy()
    {
        var strategy = MakeStrategy("HOME_V1");
        var factory  = new PricingStrategyFactory([strategy]);

        factory.Resolve("home_v1").Should().BeSameAs(strategy);
        factory.Resolve("Home_V1").Should().BeSameAs(strategy);
    }

    [Fact]
    public void Resolve_UnknownCode_ThrowsUnsupportedProductException()
    {
        var factory = new PricingStrategyFactory([MakeStrategy("HOME_V1")]);

        var act = () => factory.Resolve("UNKNOWN");
        act.Should().Throw<UnsupportedProductException>();
    }

    [Fact]
    public void Resolve_MultipleStrategies_ReturnsCorrectOne()
    {
        var home = MakeStrategy("HOME_V1");
        var auto = MakeStrategy("AUTO_V1");
        var factory = new PricingStrategyFactory([home, auto]);

        factory.Resolve("HOME_V1").Should().BeSameAs(home);
        factory.Resolve("AUTO_V1").Should().BeSameAs(auto);
    }

    [Fact]
    public void Resolve_EmptyStrategies_ThrowsUnsupportedProductException()
    {
        var factory = new PricingStrategyFactory([]);
        var act = () => factory.Resolve("HOME_V1");
        act.Should().Throw<UnsupportedProductException>();
    }
}
