using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace PricingEngine.Domain.Common;

public abstract class Specification<T>
{
    private static readonly ConcurrentDictionary<string, Func<T, bool>> DelegateCache = new();

    private readonly List<string> _cacheKey;

    protected Specification()
        => _cacheKey = new List<string> { GetType().Name };

    protected virtual bool Include => true;

    public virtual bool IsSatisfiedBy(T value)
    {
        if (!Include) return true;
        var func = DelegateCache.GetOrAdd(
            string.Join(string.Empty, _cacheKey),
            _ => ToExpression().Compile());
        return func(value);
    }

    public Specification<T> And(Specification<T> specification)
    {
        if (!specification.Include) return this;
        _cacheKey.Add($"{nameof(And)}{specification.GetType()}");
        return new BinarySpecification(this, specification, andOperator: true);
    }

    public Specification<T> Or(Specification<T> specification)
    {
        if (!specification.Include) return this;
        _cacheKey.Add($"{nameof(Or)}{specification.GetType()}");
        return new BinarySpecification(this, specification, andOperator: false);
    }

    public abstract Expression<Func<T, bool>> ToExpression();

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
