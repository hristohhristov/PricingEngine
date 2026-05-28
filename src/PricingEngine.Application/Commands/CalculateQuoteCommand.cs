using System.Text.Json;
using MediatR;
using PricingEngine.Application.Responses;

namespace PricingEngine.Application.Commands;

public record CalculateQuoteCommand(string ProductCode, JsonElement Payload)
    : IRequest<QuoteSummaryResponse>;
