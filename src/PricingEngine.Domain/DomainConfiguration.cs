using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain;

[ExcludeFromCodeCoverage]
public static class DomainConfiguration
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
        => services.AddFactories();

    private static IServiceCollection AddFactories(this IServiceCollection services)
        => services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo(typeof(IFactory<>)))
            .AsMatchingInterface()
            .WithTransientLifetime());
}
