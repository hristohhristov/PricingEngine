using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PricingEngine.Application.Behaviors;
using PricingEngine.Application.Calculators;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application;

/// <summary>
/// Extension methods that register Application-layer services with the DI container.
/// Configures MediatR, FluentValidation, pricing strategies, and the installment calculator.
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Adds all Application services to the service collection.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            })
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddScoped<PricingStrategyFactory>()
            .AddScoped<IInstallmentCalculator, InstallmentCalculator>()
            .Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(c => c.AssignableTo<IProductPricingStrategy>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}
