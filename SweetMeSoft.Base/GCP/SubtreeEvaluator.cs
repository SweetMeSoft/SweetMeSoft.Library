using System.Linq.Expressions;

namespace SweetMeSoft.Base.GCP;

/// <summary>
/// Evaluates & replaces sub-trees when first candidate is reached (top-down)
/// </summary>
public class SubtreeEvaluator : ExpressionVisitor
{
    private readonly HashSet<Expression> candidates;

    internal SubtreeEvaluator(HashSet<Expression> candidates)
    {
        this.candidates = candidates;
    }

    internal Expression Eval(Expression exp)
    {
        return Visit(exp);
    }

    public override Expression Visit(Expression exp)
    {
        if (exp == null)
        {
            return null;
        }
        if (candidates.Contains(exp))
        {
            return Evaluate(exp);
        }
        return base.Visit(exp);
    }

    private Expression Evaluate(Expression e)
    {
        if (e.NodeType == ExpressionType.Constant)
        {
            return e;
        }
        LambdaExpression lambda = Expression.Lambda(e);
        Delegate fn = lambda.Compile();
        return Expression.Constant(fn.DynamicInvoke(null), e.Type);
    }
}