using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Pricing.Exceptions;

namespace PricingEngine.Domain.Pricing.Models;

using static ModelConstants.Money;

public class Money : ValueObject
{
    private readonly decimal _amount;
    private readonly string _currency;

    public Money(decimal amount, string currency = DefaultCurrency)
    {
        Guard.AgainstNegativeAmount<InvalidMoneyException>(amount, nameof(Amount));
        Guard.AgainstOutOfRange<InvalidMoneyException>(amount, MinAmount, MaxAmount, nameof(Amount));
        Guard.AgainstEmptyString<InvalidMoneyException>(currency, nameof(Currency));
        _amount = amount;
        _currency = currency;
    }

    public decimal Amount => _amount;
    public string Currency => _currency;

    public static Money Zero(string currency = DefaultCurrency) => new(0m, currency);
    public static Money None(string currency = DefaultCurrency) => new(0m, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(_amount + other._amount, _currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = _amount - other._amount;
        if (result < 0m)
            throw new InvalidMoneyException("Subtraction would result in a negative monetary amount.");
        return new Money(result, _currency);
    }

    public Money MultiplyBy(decimal factor)
    {
        if (factor < 0m)
            throw new InvalidMoneyException("Multiplication factor cannot be negative.");
        return new Money(_amount * factor, _currency);
    }

    public Money Percentage(decimal rate)
    {
        if (rate < 0m || rate > 1m)
            throw new InvalidMoneyException("Rate must be between 0 and 1.");
        return new Money(_amount * rate, _currency);
    }

    public Money Round()
        => new Money(Math.Round(_amount, 2, MidpointRounding.AwayFromZero), _currency);

    private void EnsureSameCurrency(Money other)
    {
        if (_currency != other._currency)
            throw new InvalidMoneyException(
                $"Cannot operate on different currencies: {_currency} and {other._currency}.");
    }
}
