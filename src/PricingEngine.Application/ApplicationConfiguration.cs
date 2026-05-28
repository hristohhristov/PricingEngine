using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PricingEngine.Application.Behaviors;
using PricingEngine.Application.Calculators;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application;

public static class ApplicationConfiguration
{
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
