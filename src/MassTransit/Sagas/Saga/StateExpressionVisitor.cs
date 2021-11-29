namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;


    public class StateExpressionVisitor<TInstance> :
        ExpressionVisitor
    {
        readonly Expression _expressionBody;
        readonly ParameterExpression _instanceParameter;

        public StateExpressionVisitor(Expression<Func<TInstance, bool>> expression)
        {
            _instanceParameter = expression.Parameters[0];
            _expressionBody = expression.Body;
        }

        /// <summary>
        /// Combines the base expression with the specified state expression (from the state accessor)
        /// </summary>
        /// <param name="stateExpression">The state expression</param>
        /// <param name="not">If true, adds a not to the expression, otherwise, matches any of the states</param>
        /// <returns>The combined expression</returns>
        Expression<Func<TInstance, bool>> Combine(Expression<Func<TInstance, bool>> stateExpression, bool not = false)
        {
            var result = Visit(stateExpression);
            if (result is LambdaExpression lambda)
            {
                var stateExpressionBody = not
                    ? Expression.Not(lambda.Body)
                    : lambda.Body;

                return Expression.Lambda<Func<TInstance, bool>>(Expression.AndAlso(_expressionBody, stateExpressionBody), _instanceParameter);
            }

            throw new ArgumentException("Could not combine the expression", nameof(stateExpression));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != null && node.Type == typeof(TInstance))
                return _instanceParameter;

            return base.VisitParameter(node);
        }

        public static Expression<Func<TInstance, bool>> Combine(Expression<Func<TInstance, bool>> expression, Expression<Func<TInstance, bool>> stateExpression)
        {
            return new StateExpressionVisitor<TInstance>(expression).Combine(stateExpression);
        }
    }
}
