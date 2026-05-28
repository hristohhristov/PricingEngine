using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Infrastructure.Persistence.Configurations;

public class ProductConfigurationEntityConfiguration
    : IEntityTypeConfiguration<ProductConfiguration>
{
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

        // HasData with anonymous object bypasses the internal domain constructor
        builder.HasData(new
        {
            Id          = new Guid("a1a1a1a1-0000-0000-0000-000000000001"),
            ProductCode = "HOME_V1",
            ConfigData  = """{"BaseTariff":0.005,"FixedFee":25.00}""",
            ValidFrom   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ValidTo     = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsDeleted   = false,
            DeletedAt   = (DateTime?)null,
        });
    }
}
