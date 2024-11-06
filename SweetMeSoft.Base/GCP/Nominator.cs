using System.Linq.Expressions;

namespace SweetMeSoft.Base.GCP;

/// <summary>
/// Performs bottom-up analysis to determine which nodes can possibly
/// be part of an evaluated sub-tree.
/// </summary>
public class Nominator : ExpressionVisitor
{
    private readonly Func<Expression, bool> fnCanBeEvaluated;

    private HashSet<Expression> candidates;

    private bool cannotBeEvaluated;

    internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
    {
        this.fnCanBeEvaluated = fnCanBeEvaluated;
    }

    internal HashSet<Expression> Nominate(Expression expression)
    {
        candidates = new HashSet<Expression>();
        Visit(expression);
        return candidates;
    }

    public override Expression Visit(Expression expression)
    {
        if (expression != null)
        {
            bool saveCannotBeEvaluated = cannotBeEvaluated;
            cannotBeEvaluated = false;
            base.Visit(expression);
            if (!cannotBeEvaluated)
            {
                if (fnCanBeEvaluated(expression))
                {
                    candidates.Add(expression);
                }
                else
                {
                    cannotBeEvaluated = true;
                }
            }
            cannotBeEvaluated |= saveCannotBeEvaluated;
        }
        return expression;
    }
}