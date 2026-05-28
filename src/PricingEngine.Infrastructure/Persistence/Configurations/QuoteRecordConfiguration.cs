using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Infrastructure.Persistence.Configurations;

public class QuoteRecordConfiguration : IEntityTypeConfiguration<QuoteRecord>
{
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
        builder.Ignore(x => x.TotalAmount); // computed C# property, not a DB column

        builder.HasIndex(x => x.ProductCode);
    }
}
