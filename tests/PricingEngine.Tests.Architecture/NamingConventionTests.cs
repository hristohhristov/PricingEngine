using System.Reflection;
using FluentAssertions;
using FluentValidation;
using MassTransit;
using MediatR;
using NetArchTest.Rules;
using PricingEngine.Application.Commands;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Infrastructure.Persistence;
using PricingEngine.Web;

namespace PricingEngine.Tests.Architecture;

/// <summary>
/// Naming convention rules across all layers.
/// </summary>
public class NamingConventionTests
{
    private static readonly Assembly DomainAssembly         = typeof(IDomainEvent).Assembly;
    private static readonly Assembly ApplicationAssembly    = typeof(CalculateQuoteCommand).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(PricingDbContext).Assembly;
    private static readonly Assembly WebAssembly            = typeof(ApiController).Assembly;

    private static readonly Assembly[] AllAssemblies =
    [
        DomainAssembly, ApplicationAssembly, InfrastructureAssembly, WebAssembly
    ];

    // ── Repositories ──────────────────────────────────────────────────────────

    [Fact]
    public void Repositories_HaveRepositorySuffix()
    {
        foreach (var asm in AllAssemblies)
        {
            var result = Types.InAssembly(asm)
                .That()
                .ImplementInterface(typeof(IDomainRepository<>))
                .Should()
                .HaveNameEndingWith("Repository")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All IDomainRepository implementations in {asm.GetName().Name} must end with 'Repository'. " +
                         $"Failing: {Failing(result)}");
        }
    }

    // ── Consumers ─────────────────────────────────────────────────────────────

    [Fact]
    public void Consumers_HaveConsumerSuffix()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .HaveNameEndingWith("Consumer")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All MassTransit consumers must end with 'Consumer'. Failing: {Failing(result)}");
    }

    // ── Validators ────────────────────────────────────────────────────────────

    [Fact]
    public void Validators_HaveValidatorSuffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All FluentValidation validators must end with 'Validator'. Failing: {Failing(result)}");
    }

    // ── Factories ─────────────────────────────────────────────────────────────

    [Fact]
    public void Factories_HaveFactorySuffix()
    {
        foreach (var asm in AllAssemblies)
        {
            var result = Types.InAssembly(asm)
                .That()
                .ImplementInterface(typeof(IFactory<>))
                .Should()
                .HaveNameEndingWith("Factory")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: $"All IFactory implementations in {asm.GetName().Name} must end with 'Factory'. " +
                         $"Failing: {Failing(result)}");
        }
    }

    // ── Specifications ────────────────────────────────────────────────────────

    [Fact]
    public void Specifications_HaveSpecificationSuffix()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Specification<>))
            .Should()
            .HaveNameEndingWith("Specification")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All Specification subclasses must end with 'Specification'. Failing: {Failing(result)}");
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    [Fact]
    public void Commands_HaveCommandSuffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All IRequest implementations must end with 'Command'. Failing: {Failing(result)}");
    }

    // ── Command Handlers ──────────────────────────────────────────────────────

    [Fact]
    public void CommandHandlers_HaveHandlerSuffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All IRequestHandler implementations must end with 'Handler'. Failing: {Failing(result)}");
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static string Failing(TestResult result)
        => result.FailingTypeNames is null
            ? "(none)"
            : string.Join(", ", result.FailingTypeNames);
}
