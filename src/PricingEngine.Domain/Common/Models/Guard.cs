namespace PricingEngine.Domain.Common.Models;

public static class Guard
{
    public static void AgainstEmptyString<TException>(string value, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (!string.IsNullOrWhiteSpace(value)) return;
        ThrowException<TException>($"{name} cannot be null or empty.");
    }

    public static void ForStringLength<TException>(
        string value, int minLength, int maxLength, string name = "Value")
        where TException : BaseDomainException, new()
    {
        AgainstEmptyString<TException>(value, name);
        if (minLength <= value.Length && value.Length <= maxLength) return;
        ThrowException<TException>($"{name} must be between {minLength} and {maxLength} characters.");
    }

    public static void AgainstOutOfRange<TException>(
        int number, int min, int max, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (min <= number && number <= max) return;
        ThrowException<TException>($"{name} must be between {min} and {max}.");
    }

    public static void AgainstOutOfRange<TException>(
        decimal number, decimal min, decimal max, string name = "Value")
        where TException : BaseDomainException, new()
    {
        if (min <= number && number <= max) return;
        ThrowException<TException>($"{name} must be between {min} and {max}.");
    }

    public static void AgainstOutOfRange<TException>(
        DateTime date, DateTime min, DateTime max, string name = "Date")
        where TException : BaseDomainException, new()
    {
        if (min <= date && date <= max) return;
        ThrowException<TException>($"{name} must be between {min:yyyy-MM-dd} and {max:yyyy-MM-dd}.");
    }

    public static void AgainstNegativeAmount<TException>(decimal amount, string name = "Amount")
        where TException : BaseDomainException, new()
    {
        if (amount >= 0m) return;
        ThrowException<TException>($"{name} cannot be negative.");
    }

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
