using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PricingEngine.Application.Interfaces;
using PricingEngine.Domain.Common;
using PricingEngine.Infrastructure.Messaging;
using PricingEngine.Infrastructure.Messaging.Consumers;
using PricingEngine.Infrastructure.Messaging.Options;
using PricingEngine.Infrastructure.Persistence;
using PricingEngine.Infrastructure.Seeding;

namespace PricingEngine.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddServiceBus(configuration)
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>()
            .AddScoped<DatabaseInitializer>()
            .AddScoped<IProductConfigurationSeeder, ProductConfigurationSeeder>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PricingDbContext>((_, opts) =>
            opts.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(PricingDbContext).Assembly.FullName))
                .AddInterceptors(new SoftDeleteInterceptor()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(c => c.AssignableTo(typeof(IDomainRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    private static IServiceCollection AddServiceBus(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<PricingDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.AddConsumer<AuditQuoteConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var opts = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                cfg.Host(opts.Host, opts.VirtualHost, h =>
                {
                    h.Username(opts.Username);
                    h.Password(opts.Password);
                });

                cfg.Publish<IIntegrationEvent>(p => p.Exclude = true);
                cfg.Publish<IDomainEvent>(p => p.Exclude = true);

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
