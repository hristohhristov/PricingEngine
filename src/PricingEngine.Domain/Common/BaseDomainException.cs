namespace PricingEngine.Domain.Common;

/// <summary>
/// Abstract base class for all domain-specific exceptions.
/// Provides a structured <see cref="Error"/> property in addition to the standard exception message.
/// </summary>
public abstract class BaseDomainException : Exception
{
    /// <summary>Initialises a new instance with no message.</summary>
    protected BaseDomainException() { }

    /// <summary>Initialises a new instance with the supplied error message.</summary>
    /// <param name="error">Human-readable description of the domain rule that was violated.</param>
    protected BaseDomainException(string error) : base(error)
        => _error = error;

    private string? _error;

    /// <summary>
    /// Gets or sets the domain-specific error message.
    /// Falls back to <see cref="Exception.Message"/> when no explicit error was set.
    /// </summary>
    public string Error
    {
        get => _error ?? base.Message;
        set => _error = value;
    }
}
