using System.Reflection;

namespace PricingEngine.Domain.Common.Models;

public abstract class ValueObject
{
    private static readonly BindingFlags FieldFlags =
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

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

    public static bool operator ==(ValueObject a, ValueObject b) => a.Equals(b);
    public static bool operator !=(ValueObject a, ValueObject b) => !a.Equals(b);
}
