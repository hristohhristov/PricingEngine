using System.Text.Json;
using FluentValidation;
using PricingEngine.Application.Commands;
using ProductModelConstants = PricingEngine.Domain.Products.Models.ModelConstants;

namespace PricingEngine.Application.Validators;

/// <summary>
/// FluentValidation validator for <see cref="CalculateQuoteCommand"/>.
/// Enforces that the product code is non-empty and within the allowed length range,
/// and that the payload is a valid JSON object.
/// </summary>
public class CalculateQuoteCommandValidator : AbstractValidator<CalculateQuoteCommand>
{
    /// <summary>Configures the validation rules for <see cref="CalculateQuoteCommand"/>.</summary>
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
