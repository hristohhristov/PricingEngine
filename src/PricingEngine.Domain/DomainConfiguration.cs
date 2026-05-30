using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain;

/// <summary>
/// Extension methods that register Domain-layer services with the DI container.
/// Scans the Domain assembly and registers all <see cref="IFactory{TEntity}"/> implementations as transient dependencies.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DomainConfiguration
{
    /// <summary>
    /// Adds all Domain services (currently aggregate factories) to the service collection.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services)
        => services.AddFactories();

    private static IServiceCollection AddFactories(this IServiceCollection services)
        => services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo(typeof(IFactory<>)))
            .AsMatchingInterface()
            .WithTransientLifetime());
}
