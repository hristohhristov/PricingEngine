using System.Text.Json;
using FluentValidation;
using PricingEngine.Application.Commands;
using ProductModelConstants = PricingEngine.Domain.Products.Models.ModelConstants;

namespace PricingEngine.Application.Validators;

public class CalculateQuoteCommandValidator : AbstractValidator<CalculateQuoteCommand>
{
    public CalculateQuoteCommandValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty()
            .Length(
                ProductModelConstants.ProductConfiguration.MinProductCodeLength,
                ProductModelConstants.ProductConfiguration.MaxProductCodeLength);

        RuleFor(x => x.Payload)
            .Must(p => p.ValueKind == JsonValueKind.Object)
            .WithMessage("Payload must be a valid JSON object.");
    }
}
