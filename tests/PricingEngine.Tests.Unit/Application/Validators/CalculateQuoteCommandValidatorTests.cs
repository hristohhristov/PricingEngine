using System.Text.Json;
using FluentAssertions;
using FluentValidation.TestHelper;
using PricingEngine.Application.Commands;
using PricingEngine.Application.Validators;

namespace PricingEngine.Tests.Unit.Application.Validators;

public class CalculateQuoteCommandValidatorTests
{
    private readonly CalculateQuoteCommandValidator _validator = new();

    private static JsonElement ValidPayload()
        => JsonDocument.Parse("""{"insuredSum":100000}""").RootElement;

    private static JsonElement StringPayload()
        => JsonDocument.Parse("\"hello\"").RootElement;

    // ── ProductCode ──────────────────────────────────────────────────────────

    [Fact]
    public void ProductCode_Empty_FailsValidation()
    {
        var cmd    = new CalculateQuoteCommand("", ValidPayload());
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductCode);
    }

    [Fact]
    public void ProductCode_TooShort_FailsValidation()
    {
        var cmd    = new CalculateQuoteCommand("AB", ValidPayload()); // min is 3
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductCode);
    }

    [Fact]
    public void ProductCode_TooLong_FailsValidation()
    {
        var longCode = new string('X', 51);  // max is 50
        var cmd      = new CalculateQuoteCommand(longCode, ValidPayload());
        var result   = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductCode);
    }

    [Fact]
    public void ProductCode_MinLength_PassesValidation()
    {
        var cmd    = new CalculateQuoteCommand("ABC", ValidPayload());
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCode);
    }

    [Fact]
    public void ProductCode_MaxLength_PassesValidation()
    {
        var maxCode = new string('X', 50);
        var cmd     = new CalculateQuoteCommand(maxCode, ValidPayload());
        var result  = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCode);
    }

    [Fact]
    public void ProductCode_ValidLength_PassesValidation()
    {
        var cmd    = new CalculateQuoteCommand("HOME_V1", ValidPayload());
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCode);
    }

    // ── Payload ──────────────────────────────────────────────────────────────

    [Fact]
    public void Payload_NotAnObject_FailsWithMessage()
    {
        var cmd    = new CalculateQuoteCommand("HOME_V1", StringPayload());
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Payload)
            .WithErrorMessage("Payload must be a valid JSON object.");
    }

    [Fact]
    public void Payload_ValidObject_PassesValidation()
    {
        var cmd    = new CalculateQuoteCommand("HOME_V1", ValidPayload());
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Payload);
    }

    [Fact]
    public void Payload_EmptyObject_PassesValidation()
    {
        var empty  = JsonDocument.Parse("{}").RootElement;
        var cmd    = new CalculateQuoteCommand("HOME_V1", empty);
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Payload);
    }

    // ── All fields invalid ───────────────────────────────────────────────────

    [Fact]
    public void InvalidCommand_ReturnsMultipleErrors()
    {
        var cmd    = new CalculateQuoteCommand("", StringPayload());
        var result = _validator.TestValidate(cmd);
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
