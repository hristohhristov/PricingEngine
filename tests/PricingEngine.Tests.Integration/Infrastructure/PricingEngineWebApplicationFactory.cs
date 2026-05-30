using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using PricingEngine.Application.Interfaces;
using PricingEngine.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace PricingEngine.Tests.Integration.Infrastructure;

/// <summary>
/// Shared fixture that:
///   1. Starts a Docker SQL Server container (once per test class via IClassFixture).
///   2. Boots the real ASP.NET Core application (all middleware, DI, routing).
///   3. Replaces the DbContext connection string with the test container.
///   4. Removes MassTransit hosted services so no RabbitMQ connection is attempted.
///   5. Replaces IIntegrationEventPublisher with a scoped NSubstitute mock.
/// </summary>
public class PricingEngineWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    // Exposed so IntegrationTestBase can open raw connections for table cleanup.
    public string ConnectionString => _sqlContainer.GetConnectionString();

    // Exposed so tests can assert on Received() calls.
    public IIntegrationEventPublisher EventPublisher { get; } =
        Substitute.For<IIntegrationEventPublisher>();

    // ── IAsyncLifetime ────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }

    // ── WebApplicationFactory override ────────────────────────────────────────

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remove the original DbContextOptions registered by AddDatabase().
            var dbDesc = services.Single(
                s => s.ServiceType == typeof(DbContextOptions<PricingDbContext>));
            services.Remove(dbDesc);

            // 2. Re-register pointing to the test container, preserving SoftDeleteInterceptor.
            services.AddDbContext<PricingDbContext>((_, opts) =>
                opts.UseSqlServer(
                        ConnectionString,
                        sql => sql.MigrationsAssembly(typeof(PricingDbContext).Assembly.FullName))
                    .AddInterceptors(new SoftDeleteInterceptor()));

            // 3. Remove MassTransit IHostedService registrations to prevent RabbitMQ connections.
            var massTransitServices = services
                .Where(s =>
                    s.ServiceType == typeof(IHostedService) &&
                    s.ImplementationType?.Assembly?.GetName().Name?.StartsWith("MassTransit") == true)
                .ToList();

            foreach (var svc in massTransitServices)
                services.Remove(svc);

            // 4. Replace the scoped IIntegrationEventPublisher with a captured mock.
            var pubDesc = services.SingleOrDefault(
                s => s.ServiceType == typeof(IIntegrationEventPublisher));
            if (pubDesc is not null)
                services.Remove(pubDesc);

            services.AddScoped<IIntegrationEventPublisher>(_ => EventPublisher);
        });
    }
}
