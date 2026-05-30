namespace PricingEngine.Domain.Common.Models;

/// <summary>
/// Static helper that enforces domain invariants by throwing typed <see cref="BaseDomainException"/> subclasses.
/// All methods are no-ops when the supplied value satisfies the guard condition.
/// </summary>
public static class Guard
{
    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="value"/> is null, empty, or whitespace.</summary>
    /// <typeparam name="TException">The domain exception type to throw; must have a <c>string</c> constructor.</typeparam>
    /// <param name="value">The string to validate.</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public static void AgainstEmptyString<TException>(string value, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (!string.IsNullOrWhiteSpace(value)) return;
        ThrowException<TException>($"{name} cannot be null or empty.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="value"/> is empty/whitespace or outside the specified length bounds.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="value">The string to validate.</param>
    /// <param name="minLength">Minimum allowed length (inclusive).</param>
    /// <param name="maxLength">Maximum allowed length (inclusive).</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when the string is empty or its length is out of range.</exception>
    public static void ForStringLength<TException>(
        string value, int minLength, int maxLength, string name = "Value")
        where TException : BaseDomainException, new()
    {
        AgainstEmptyString<TException>(value, name);
        if (minLength <= value.Length && value.Length <= maxLength) return;
        ThrowException<TException>($"{name} must be between {minLength} and {maxLength} characters.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="number"/> is outside the specified integer range.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="number">The integer value to validate.</param>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="number"/> is outside [<paramref name="min"/>, <paramref name="max"/>].</exception>
    public static void AgainstOutOfRange<TException>(
        int number, int min, int max, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (min <= number && number <= max) return;
        ThrowException<TException>($"{name} must be between {min} and {max}.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="number"/> is outside the specified decimal range.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="number">The decimal value to validate.</param>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="number"/> is outside [<paramref name="min"/>, <paramref name="max"/>].</exception>
    public static void AgainstOutOfRange<TException>(
        decimal number, decimal min, decimal max, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (min <= number && number <= max) return;
        ThrowException<TException>($"{name} must be between {min} and {max}.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="date"/> falls outside the specified date range.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="date">The date value to validate.</param>
    /// <param name="min">Minimum allowed date (inclusive).</param>
    /// <param name="max">Maximum allowed date (inclusive).</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="date"/> is outside [<paramref name="min"/>, <paramref name="max"/>].</exception>
    public static void AgainstOutOfRange<TException>(
        DateTime date, DateTime min, DateTime max, string name = "Date")
        where TException : BaseDomainException, new()
    {
        if (min <= date && date <= max) return;
        ThrowException<TException>($"{name} must be between {min:yyyy-MM-dd} and {max:yyyy-MM-dd}.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="amount"/> is negative.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="amount">The monetary amount to validate.</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="amount"/> is less than zero.</exception>
    public static void AgainstNegativeAmount<TException>(decimal amount, string name = "Amount")
        where TException : BaseDomainException, new()
    {
        if (amount >= 0m) return;
        ThrowException<TException>($"{name} cannot be negative.");
    }

    /// <summary>Throws <typeparamref name="TException"/> if <paramref name="count"/> is not 1, 2, or 4.</summary>
    /// <typeparam name="TException">The domain exception type to throw.</typeparam>
    /// <param name="count">The installment count to validate.</param>
    /// <param name="name">The display name of the field used in the error message.</param>
    /// <exception cref="BaseDomainException">Thrown when <paramref name="count"/> is not a valid installment count.</exception>
    public static void ForValidInstallmentCount<TException>(int count, string name = "InstallmentCount")
        where TException : BaseDomainException, new()
    {
        if (count == 1 || count == 2 || count == 4) return;
        ThrowException<TException>($"{name} must be 1, 2, or 4.");
    }

    private static void ThrowException<TException>(string message)
        where TException : BaseDomainException, new()
    {
        var ex = (TException)Activator.CreateInstance(typeof(TException), message)!;
        throw ex;
    }
}
