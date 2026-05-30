using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace PricingEngine.Domain.Common;

/// <summary>
/// Abstract base class for the Specification pattern.
/// Supports composing specifications with <see cref="And"/> and <see cref="Or"/>, and exposes the predicate as an <see cref="Expression{TDelegate}"/> for LINQ-to-SQL translation.
/// </summary>
/// <typeparam name="T">The entity type the specification applies to.</typeparam>
public abstract class Specification<T>
{
    private static readonly ConcurrentDictionary<string, Func<T, bool>> DelegateCache = new();

    private readonly List<string> _cacheKey;

    /// <summary>Initialises the specification and seeds the cache key with the concrete type name.</summary>
    protected Specification()
        => _cacheKey = new List<string> { GetType().Name };

    /// <summary>
    /// When <c>false</c> the specification is treated as always satisfied and is omitted from composite expressions.
    /// Override to make a specification conditional.
    /// </summary>
    protected virtual bool Include => true;

    /// <summary>
    /// Evaluates the specification against the given value using a compiled and cached delegate.
    /// </summary>
    /// <param name="value">The candidate entity to test.</param>
    /// <returns><c>true</c> when the entity satisfies this specification; otherwise <c>false</c>.</returns>
    public virtual bool IsSatisfiedBy(T value)
    {
        if (!Include) return true;
        var func = DelegateCache.GetOrAdd(
            string.Join(string.Empty, _cacheKey),
            _ => ToExpression().Compile());
        return func(value);
    }

    /// <summary>
    /// Combines this specification with another using a logical AND.
    /// The resulting specification is satisfied only when both operands are satisfied.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new composite specification representing the AND of both.</returns>
    public Specification<T> And(Specification<T> specification)
    {
        if (!specification.Include) return this;
        _cacheKey.Add($"{nameof(And)}{specification.GetType()}");
        return new BinarySpecification(this, specification, andOperator: true);
    }

    /// <summary>
    /// Combines this specification with another using a logical OR.
    /// The resulting specification is satisfied when either operand is satisfied.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new composite specification representing the OR of both.</returns>
    public Specification<T> Or(Specification<T> specification)
    {
        if (!specification.Include) return this;
        _cacheKey.Add($"{nameof(Or)}{specification.GetType()}");
        return new BinarySpecification(this, specification, andOperator: false);
    }

    /// <summary>
    /// Returns the predicate expression that represents this specification.
    /// Used by LINQ providers (e.g., EF Core) to translate the filter to SQL.
    /// </summary>
    /// <returns>A lambda expression that tests whether <typeparamref name="T"/> satisfies the specification.</returns>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Implicitly converts a specification to its equivalent LINQ expression.
    /// Returns a pass-through expression (<c>_ =&gt; true</c>) when <see cref="Include"/> is <c>false</c>.
    /// </summary>
    /// <param name="specification">The specification to convert.</param>
    /// <returns>The specification's predicate expression.</returns>
    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        => specification.Include ? specification.ToExpression() : _ => true;

    private sealed class BinarySpecification : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;
        private readonly bool _andOperator;

        internal BinarySpecification(Specification<T> left, Specification<T> right, bool andOperator)
        {
            _left = left;
            _right = right;
            _andOperator = andOperator;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpr = _left;
            Expression<Func<T, bool>> rightExpr = _right;

            Expression body = _andOperator
                ? Expression.AndAlso(leftExpr.Body, rightExpr.Body)
                : Expression.OrElse(leftExpr.Body, rightExpr.Body);

            var param = Expression.Parameter(typeof(T));
            body = new ParameterReplacer(param).Visit(body)!;

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly HashSet<ParameterExpression> _bound = new();

        internal ParameterReplacer(ParameterExpression parameter)
            => _parameter = parameter;

        protected override Expression VisitLambda<TL>(Expression<TL> node)
        {
            foreach (var p in node.Parameters) _bound.Add(p);
            var visited = base.VisitLambda(node);
            foreach (var p in node.Parameters) _bound.Remove(p);
            return visited;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => _bound.Contains(node) ? node : _parameter;
    }
}
