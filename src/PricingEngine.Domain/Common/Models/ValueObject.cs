using System.Reflection;

namespace PricingEngine.Domain.Common.Models;

/// <summary>
/// Abstract base class for DDD value objects.
/// Equality is determined by structural field comparison via reflection.
/// </summary>
public abstract class ValueObject
{
    private static readonly BindingFlags FieldFlags =
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// Determines whether the specified object is structurally equal to this value object.
    /// Two instances are equal when they have the same type and all instance fields are equal.
    /// </summary>
    /// <param name="other">The object to compare with this instance.</param>
    /// <returns><c>true</c> when structurally equal; otherwise <c>false</c>.</returns>
    public override bool Equals(object? other)
    {
        if (other is null) return false;
        if (GetType() != other.GetType()) return false;

        foreach (var field in GetType().GetFields(FieldFlags))
        {
            var a = field.GetValue(other);
            var b = field.GetValue(this);
            if (a is null && b is not null) return false;
            if (a is not null && !a.Equals(b)) return false;
        }
        return true;
    }

    /// <summary>
    /// Returns a hash code computed from all instance fields across the entire class hierarchy.
    /// </summary>
    /// <returns>A composite hash code based on all field values.</returns>
    public override int GetHashCode()
    {
        const int start = 17, multiplier = 59;
        return GetAllFields()
            .Select(f => f.GetValue(this))
            .Where(v => v is not null)
            .Aggregate(start, (h, v) => h * multiplier + v!.GetHashCode());
    }

    private IEnumerable<FieldInfo> GetAllFields()
    {
        var type = GetType();
        var fields = new List<FieldInfo>();
        while (type != typeof(object) && type is not null)
        {
            fields.AddRange(type.GetFields(FieldFlags));
            type = type.BaseType!;
        }
        return fields;
    }

    /// <summary>Returns <c>true</c> when both value objects are structurally equal.</summary>
    /// <param name="a">Left operand.</param>
    /// <param name="b">Right operand.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(ValueObject a, ValueObject b) => a.Equals(b);

    /// <summary>Returns <c>true</c> when the two value objects are not structurally equal.</summary>
    /// <param name="a">Left operand.</param>
    /// <param name="b">Right operand.</param>
    /// <returns><c>true</c> when not equal; otherwise <c>false</c>.</returns>
    public static bool operator !=(ValueObject a, ValueObject b) => !a.Equals(b);
}
