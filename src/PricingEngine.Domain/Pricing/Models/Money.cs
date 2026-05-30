using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Pricing.Exceptions;

namespace PricingEngine.Domain.Pricing.Models;

using static ModelConstants.Money;

/// <summary>
/// Immutable value object representing a monetary amount in a specific currency.
/// All arithmetic operations return new <see cref="Money"/> instances and enforce currency consistency.
/// </summary>
public class Money : ValueObject
{
    private readonly decimal _amount;
    private readonly string _currency;

    /// <summary>
    /// Initialises a new monetary value, validating that the amount is non-negative and within range
    /// and that the currency code is non-empty.
    /// </summary>
    /// <param name="amount">The monetary amount; must be between <see cref="MinAmount"/> and <see cref="MaxAmount"/>.</param>
    /// <param name="currency">ISO 4217 currency code; defaults to <see cref="DefaultCurrency"/> ("EUR").</param>
    /// <exception cref="InvalidMoneyException">Thrown when amount is negative, out of range, or currency is empty.</exception>
    public Money(decimal amount, string currency = DefaultCurrency)
    {
        Guard.AgainstNegativeAmount<InvalidMoneyException>(amount, nameof(Amount));
        Guard.AgainstOutOfRange<InvalidMoneyException>(amount, MinAmount, MaxAmount, nameof(Amount));
        Guard.AgainstEmptyString<InvalidMoneyException>(currency, nameof(Currency));
        _amount = amount;
        _currency = currency;
    }

    /// <summary>Gets the monetary amount.</summary>
    public decimal Amount => _amount;

    /// <summary>Gets the ISO 4217 currency code.</summary>
    public string Currency => _currency;

    /// <summary>Returns a zero-amount <see cref="Money"/> in the specified currency.</summary>
    /// <param name="currency">ISO 4217 currency code; defaults to <see cref="DefaultCurrency"/>.</param>
    /// <returns>A <see cref="Money"/> instance with amount 0.00.</returns>
    public static Money Zero(string currency = DefaultCurrency) => new(0m, currency);

    /// <summary>Alias for <see cref="Zero(string)"/>; returns a zero-amount instance in the specified currency.</summary>
    /// <param name="currency">ISO 4217 currency code; defaults to <see cref="DefaultCurrency"/>.</param>
    /// <returns>A <see cref="Money"/> instance with amount 0.00.</returns>
    public static Money None(string currency = DefaultCurrency) => new(0m, currency);

    /// <summary>
    /// Adds another monetary amount to this instance.
    /// Both operands must share the same currency.
    /// </summary>
    /// <param name="other">The amount to add.</param>
    /// <returns>A new <see cref="Money"/> equal to the sum.</returns>
    /// <exception cref="InvalidMoneyException">Thrown when currencies differ.</exception>
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(_amount + other._amount, _currency);
    }

    /// <summary>
    /// Subtracts another monetary amount from this instance.
    /// Both operands must share the same currency and the result must not be negative.
    /// </summary>
    /// <param name="other">The amount to subtract.</param>
    /// <returns>A new <see cref="Money"/> equal to the difference.</returns>
    /// <exception cref="InvalidMoneyException">Thrown when currencies differ or the result would be negative.</exception>
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = _amount - other._amount;
        if (result < 0m)
            throw new InvalidMoneyException("Subtraction would result in a negative monetary amount.");
        return new Money(result, _currency);
    }

    /// <summary>
    /// Multiplies this amount by a non-negative scalar factor.
    /// </summary>
    /// <param name="factor">The multiplication factor; must be zero or positive.</param>
    /// <returns>A new <see cref="Money"/> equal to the product.</returns>
    /// <exception cref="InvalidMoneyException">Thrown when <paramref name="factor"/> is negative.</exception>
    public Money MultiplyBy(decimal factor)
    {
        if (factor < 0m)
            throw new InvalidMoneyException("Multiplication factor cannot be negative.");
        return new Money(_amount * factor, _currency);
    }

    /// <summary>
    /// Computes a percentage of this amount, where <paramref name="rate"/> is a value between 0 and 1.
    /// </summary>
    /// <param name="rate">The percentage rate expressed as a decimal (e.g., 0.10 for 10%).</param>
    /// <returns>A new <see cref="Money"/> equal to this amount multiplied by the rate.</returns>
    /// <exception cref="InvalidMoneyException">Thrown when <paramref name="rate"/> is outside [0, 1].</exception>
    public Money Percentage(decimal rate)
    {
        if (rate < 0m || rate > 1m)
            throw new InvalidMoneyException("Rate must be between 0 and 1.");
        return new Money(_amount * rate, _currency);
    }

    /// <summary>
    /// Rounds this amount to two decimal places using midpoint rounding away from zero.
    /// </summary>
    /// <returns>A new <see cref="Money"/> with the rounded amount.</returns>
    public Money Round()
        => new Money(Math.Round(_amount, 2, MidpointRounding.AwayFromZero), _currency);

    private void EnsureSameCurrency(Money other)
    {
        if (_currency != other._currency)
            throw new InvalidMoneyException(
                $"Cannot operate on different currencies: {_currency} and {other._currency}.");
    }
}
