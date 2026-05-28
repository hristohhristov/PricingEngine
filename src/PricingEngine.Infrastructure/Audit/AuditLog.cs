namespace PricingEngine.Infrastructure.Audit;

public class AuditLog
{
    public Guid     Id          { get; set; }
    public Guid     QuoteId     { get; set; }
    public string   ProductCode { get; set; } = default!;
    public DateTime QuoteDate   { get; set; }
    public decimal  TotalAmount { get; set; }
    public string   Currency    { get; set; } = default!;
    public DateTime ProcessedAt { get; set; }
    public Guid     EventId     { get; set; }
}
