using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Products.Factories;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Infrastructure.Seeding;

public sealed class ProductConfigurationSeeder(
    IEnumerable<IProductDefinition> definitions,
    IProductConfigurationFactory factory,
    PricingDbContext dbContext,
    ILogger<ProductConfigurationSeeder> logger)
    : IProductConfigurationSeeder
{
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
