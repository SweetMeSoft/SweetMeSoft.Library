using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SweetMeSoft.Base.GCP
{
    public class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder sb;

        public int? Skip { get; set; }

        public int? Take { get; set; }

        public string OrderBy { get; set; }

        public QueryTranslator()
        {
        }

        public string Translate(Expression expression)
        {
            sb = new StringBuilder();
            Visit(expression);
            return sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                return m;
            }

            if (m.Method.Name == "Take")
            {
                if (ParseTakeExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }
            }

            if (m.Method.Name == "Skip")
            {
                if (ParseSkipExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }
            }

            if (m.Method.Name == "OrderBy")
            {
                if (ParseOrderByExpression(m, "ASC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }
            }

            if (m.Method.Name == "OrderByDescending")
            {
                if (ParseOrderByExpression(m, "DESC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }
            }

            if (m.Method.Name == "StartsWith")
            {
                Visit(m.Object);
                var a = (ConstantExpression)m.Arguments[0];
                sb.Append(" LIKE '" + a.Value.ToString().Replace("\"", "") + "%' ");
                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                AddValue(c.Value);
            }

            return c;
        }

        private void AddValue(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    sb.Append((bool)value ? 1 : 0);
                    break;
                case TypeCode.String:
                    sb.Append("'" + value.ToString().Replace("\"", "") + "'");
                    break;
                case TypeCode.DateTime:
                    sb.Append("'" + DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");
                    break;
                case TypeCode.Object:
                    if (Guid.TryParse(value.ToString(), out var id))
                    {
                        sb.Append("'" + id.ToString().Replace("\"", "") + "'");
                        break;
                    }

                    throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                default:
                    sb.Append(value);
                    break;
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }

            if (m.Expression.NodeType == ExpressionType.Constant)
            {
                object container = ((ConstantExpression)m.Expression).Value;
                var member = m.Member;
                if (member is FieldInfo info)
                {
                    object value = info.GetValue(container);
                    AddValue(value);
                    return Expression.Constant(value);
                }
                if (member is PropertyInfo info1)
                {
                    object value = info1.GetValue(container, null);
                    AddValue(value);
                    return Expression.Constant(value);
                }
            }

            if (m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                object value = Expression.Lambda<Func<object>>(Expression.Convert(m, typeof(object))).Compile().Invoke();
                sb.Append("'" + value + "'");
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            var unary = (UnaryExpression)expression.Arguments[1];
            var lambdaExpression = (LambdaExpression)unary.Operand;
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            if (lambdaExpression.Body is MemberExpression body)
            {
                if (string.IsNullOrEmpty(OrderBy))
                {
                    OrderBy = string.Format("{0} {1}", body.Member.Name, order);
                }
                else
                {
                    OrderBy = string.Format("{0}, {1} {2}", OrderBy, body.Member.Name, order);
                }

                return true;
            }

            return false;
        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (int.TryParse(sizeExpression.Value.ToString(), out int size))
            {
                Take = size;
                return true;
            }

            return false;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (int.TryParse(sizeExpression.Value.ToString(), out int size))
            {
                Skip = size;
                return true;
            }

            return false;
        }
    }
}