namespace PricingEngine.Infrastructure.Seeding;

public interface IProductConfigurationSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
