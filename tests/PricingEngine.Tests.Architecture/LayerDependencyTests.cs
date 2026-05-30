using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using PricingEngine.Domain.Common;
using PricingEngine.Application.Commands;
using PricingEngine.Infrastructure.Persistence;
using PricingEngine.Web;

namespace PricingEngine.Tests.Architecture;

/// <summary>
/// Enforces clean architecture layering:
///   Domain  ← Application ← Infrastructure ← Web
/// No layer may depend on a layer above it.
/// </summary>
public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly         = typeof(IDomainEvent).Assembly;
    private static readonly Assembly ApplicationAssembly    = typeof(CalculateQuoteCommand).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(PricingDbContext).Assembly;
    private static readonly Assembly WebAssembly            = typeof(ApiController).Assembly;

    private const string DomainNs         = "PricingEngine.Domain";
    private const string ApplicationNs    = "PricingEngine.Application";
    private const string InfrastructureNs = "PricingEngine.Infrastructure";
    private const string WebNs            = "PricingEngine.Web";

    // ── Domain layer ──────────────────────────────────────────────────────────

    [Fact]
    public void Domain_DoesNotDependOn_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain layer must not reference Application. Failing types: {Failing(result)}");
    }

    [Fact]
    public void Domain_DoesNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain layer must not reference Infrastructure. Failing types: {Failing(result)}");
    }

    [Fact]
    public void Domain_DoesNotDependOn_Web()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(WebNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain layer must not reference Web. Failing types: {Failing(result)}");
    }

    // ── Application layer ─────────────────────────────────────────────────────

    [Fact]
    public void Application_DoesNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application layer must not reference Infrastructure. Failing types: {Failing(result)}");
    }

    [Fact]
    public void Application_DoesNotDependOn_Web()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(WebNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application layer must not reference Web. Failing types: {Failing(result)}");
    }

    // ── Infrastructure layer ──────────────────────────────────────────────────

    [Fact]
    public void Infrastructure_DoesNotDependOn_Web()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(WebNs)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Infrastructure layer must not reference Web. Failing types: {Failing(result)}");
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static string Failing(TestResult result)
        => result.FailingTypeNames is null
            ? "(none)"
            : string.Join(", ", result.FailingTypeNames);
}
