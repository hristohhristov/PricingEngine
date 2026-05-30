using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core fluent configuration for the <see cref="QuoteRecord"/> aggregate root.
/// Maps to the <c>QuoteRecords</c> table and configures column precision, status conversion, and indexes.
/// </summary>
public class QuoteRecordConfiguration : IEntityTypeConfiguration<QuoteRecord>
{
    /// <summary>
    /// Applies the entity type mapping configuration for <see cref="QuoteRecord"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<QuoteRecord> builder)
    {
        builder.ToTable("QuoteRecords");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.QuoteDate).IsRequired();
        builder.Property(x => x.NetPremium).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.TotalAmount);

        builder.HasIndex(x => x.ProductCode);
    }
}
