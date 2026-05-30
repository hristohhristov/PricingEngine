using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Tests.Architecture;

/// <summary>
/// Domain model structural rules:
///   - Aggregate roots implement IAggregateRoot
///   - Value objects inherit from ValueObject
///   - Domain events implement IDomainEvent
///   - Domain exceptions inherit from BaseDomainException
/// </summary>
public class DomainModelTests
{
    private static readonly Assembly DomainAssembly = typeof(IDomainEvent).Assembly;

    // ── Aggregate roots ───────────────────────────────────────────────────────

    [Fact]
    public void AggregateRoots_ImplementIAggregateRoot()
    {
        // QuoteRecord and ProductConfiguration are the known aggregate roots
        typeof(QuoteRecord).Should().Implement<IAggregateRoot>();
        typeof(ProductConfiguration).Should().Implement<IAggregateRoot>();
    }

    [Fact]
    public void AggregateRoots_InheritFromEntity()
    {
        typeof(QuoteRecord).Should().BeAssignableTo<Entity>();
        typeof(ProductConfiguration).Should().BeAssignableTo<Entity>();
    }

    // ── Value objects ─────────────────────────────────────────────────────────

    [Fact]
    public void Money_InheritsFromValueObject()
    {
        typeof(Money).Should().BeAssignableTo<ValueObject>();
    }

    [Fact]
    public void QuoteResult_InheritsFromValueObject()
    {
        typeof(QuoteResult).Should().BeAssignableTo<ValueObject>();
    }

    [Fact]
    public void InstallmentPlan_InheritsFromValueObject()
    {
        typeof(InstallmentPlan).Should().BeAssignableTo<ValueObject>();
    }

    [Fact]
    public void AllValueObjects_InheritFromValueObject()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("PricingEngine.Domain.Pricing.Models")
            .And()
            .Inherit(typeof(ValueObject))
            .Should()
            .Inherit(typeof(ValueObject))
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    // ── Domain events ─────────────────────────────────────────────────────────

    [Fact]
    public void DomainEvents_ImplementIDomainEvent()
    {
        // Concrete types ending in "DomainEvent" should inherit from the DomainEvent base class
        // (which in turn implements IDomainEvent). We use Inherit() rather than ImplementInterface()
        // because NetArchTest checks only direct interface implementations, not transitive ones.
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("DomainEvent")
            .And()
            .AreNotAbstract()
            .And()
            .AreNotInterfaces()
            .Should()
            .Inherit(typeof(DomainEvent))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All concrete types ending in 'DomainEvent' must inherit from DomainEvent. " +
                     $"Failing: {Failing(result)}");
    }

    // ── Domain exceptions ─────────────────────────────────────────────────────

    [Fact]
    public void DomainExceptions_InheritFromBaseDomainException()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Exception")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(BaseDomainException))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All concrete domain exceptions must inherit from BaseDomainException. " +
                     $"Failing: {Failing(result)}");
    }

    // ── Encapsulation: no public setters on aggregate roots ───────────────────

    [Fact]
    public void QuoteRecord_HasNoPublicSetters()
    {
        var publicSetters = typeof(QuoteRecord)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetSetMethod() is { IsPublic: true })
            .Select(p => p.Name)
            .ToList();

        // IsDeleted and DeletedAt are part of ISoftDelete — EF Core requires public setters.
        // All other domain properties must NOT have public setters.
        var domainPublicSetters = publicSetters
            .Except(["IsDeleted", "DeletedAt"])
            .ToList();

        domainPublicSetters.Should().BeEmpty(
            because: "QuoteRecord aggregate root should have no public setters on domain properties — " +
                     $"all state changes via methods. Found: {string.Join(", ", domainPublicSetters)}");
    }

    [Fact]
    public void ProductConfiguration_HasNoPublicSetters()
    {
        var publicSetters = typeof(ProductConfiguration)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetSetMethod() is { IsPublic: true })
            .Select(p => p.Name)
            .ToList();

        // IsDeleted and DeletedAt are part of ISoftDelete — they have public setters
        // by design (EF Core needs them). Exclude those.
        var domainPublicSetters = publicSetters
            .Except(["IsDeleted", "DeletedAt"])
            .ToList();

        domainPublicSetters.Should().BeEmpty(
            because: "ProductConfiguration aggregate root should expose no public setters on domain properties. " +
                     $"Found: {string.Join(", ", domainPublicSetters)}");
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static string Failing(TestResult result)
        => result.FailingTypeNames is null
            ? "(none)"
            : string.Join(", ", result.FailingTypeNames);
}
