using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core fluent configuration for the <see cref="ProductConfiguration"/> aggregate root.
/// Maps to the <c>ProductConfigurations</c> table and applies column constraints, soft-delete defaults, and indexes.
/// </summary>
public class ProductConfigurationEntityConfiguration
    : IEntityTypeConfiguration<ProductConfiguration>
{
    /// <summary>
    /// Applies the entity type mapping configuration for <see cref="ProductConfiguration"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ProductConfiguration> builder)
    {
        builder.ToTable("ProductConfigurations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ConfigData).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.ValidFrom).IsRequired();
        builder.Property(x => x.ValidTo).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => x.ProductCode);
    }
}
