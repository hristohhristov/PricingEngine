using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Products.Factories;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Infrastructure.Seeding;

/// <summary>
/// Seeds a default <c>ProductConfiguration</c> row for each registered <see cref="IProductDefinition"/>
/// that does not yet have one in the database.
/// </summary>
public sealed class ProductConfigurationSeeder(
    IEnumerable<IProductDefinition> definitions,
    IProductConfigurationFactory factory,
    PricingDbContext dbContext,
    ILogger<ProductConfigurationSeeder> logger)
    : IProductConfigurationSeeder
{
    /// <summary>
    /// Iterates over all product definitions and inserts a configuration row for any that are missing.
    /// Existing configurations (including soft-deleted ones) are skipped.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all seed operations have finished.</returns>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var definition in definitions)
        {
            var code = definition.SupportedProductCode;

            var exists = await dbContext.ProductConfigurations
                .IgnoreQueryFilters()
                .AnyAsync(p => p.ProductCode == code, cancellationToken);

            if (exists)
            {
                logger.LogDebug("Config for '{ProductCode}' already exists — skipping.", code);
                continue;
            }

            var entity = factory
                .WithProductCode(code)
                .WithConfigData(definition.DefaultConfigJson)
                .WithValidFrom(definition.DefaultValidFrom)
                .WithValidTo(definition.DefaultValidTo)
                .Build();

            await dbContext.ProductConfigurations.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded default config for '{ProductCode}'.", code);
        }
    }
}
