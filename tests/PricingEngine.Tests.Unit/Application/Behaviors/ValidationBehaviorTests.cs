using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using PricingEngine.Application.Behaviors;

namespace PricingEngine.Tests.Unit.Application.Behaviors;

// Must be public so NSubstitute/CastleDynamicProxy can intercept IValidator<ValidationTestRequest>
public record ValidationTestRequest(string Value) : IRequest<string>;

public class ValidationBehaviorTests
{

    // ── No validators ─────────────────────────────────────────────────────────

    [Fact]
    public async Task NoValidators_CallsNext()
    {
        var behavior   = new ValidationBehavior<ValidationTestRequest, string>([]);
        var nextCalled = false;

        // MediatR v12: RequestHandlerDelegate<T> takes a CancellationToken
        await behavior.Handle(
            new ValidationTestRequest("ok"),
            ct => { nextCalled = true; return Task.FromResult("result"); },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    // ── All pass ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task AllValidatorsPass_CallsNext()
    {
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        validator.Validate(Arg.Any<ValidationContext<ValidationTestRequest>>())
                 .Returns(new ValidationResult());

        var behavior   = new ValidationBehavior<ValidationTestRequest, string>([validator]);
        var nextCalled = false;

        await behavior.Handle(
            new ValidationTestRequest("ok"),
            ct => { nextCalled = true; return Task.FromResult("result"); },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    // ── Validator fails ───────────────────────────────────────────────────────

    [Fact]
    public async Task ValidatorFails_ThrowsValidationException()
    {
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        validator.Validate(Arg.Any<ValidationContext<ValidationTestRequest>>())
                 .Returns(new ValidationResult([new ValidationFailure("Value", "bad")]));

        var behavior = new ValidationBehavior<ValidationTestRequest, string>([validator]);

        var act = async () => await behavior.Handle(
            new ValidationTestRequest("bad"),
            ct => Task.FromResult("never"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    // ── Next is NOT called on failure ─────────────────────────────────────────

    [Fact]
    public async Task ValidatorFails_NextIsNotCalled()
    {
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        validator.Validate(Arg.Any<ValidationContext<ValidationTestRequest>>())
                 .Returns(new ValidationResult([new ValidationFailure("Value", "bad")]));

        var behavior   = new ValidationBehavior<ValidationTestRequest, string>([validator]);
        var nextCalled = false;

        try
        {
            await behavior.Handle(
                new ValidationTestRequest("bad"),
                ct => { nextCalled = true; return Task.FromResult("never"); },
                CancellationToken.None);
        }
        catch (ValidationException) { /* expected */ }

        nextCalled.Should().BeFalse();
    }

    // ── Multiple validators collect all failures ──────────────────────────────

    [Fact]
    public async Task MultipleValidators_CollectsAllFailures()
    {
        var v1 = Substitute.For<IValidator<ValidationTestRequest>>();
        v1.Validate(Arg.Any<ValidationContext<ValidationTestRequest>>())
          .Returns(new ValidationResult([new ValidationFailure("Value", "error1")]));

        var v2 = Substitute.For<IValidator<ValidationTestRequest>>();
        v2.Validate(Arg.Any<ValidationContext<ValidationTestRequest>>())
          .Returns(new ValidationResult([new ValidationFailure("Value", "error2")]));

        var behavior = new ValidationBehavior<ValidationTestRequest, string>([v1, v2]);

        var act = async () => await behavior.Handle(
            new ValidationTestRequest("bad"),
            ct => Task.FromResult("never"),
            CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().HaveCount(2);
    }
}
