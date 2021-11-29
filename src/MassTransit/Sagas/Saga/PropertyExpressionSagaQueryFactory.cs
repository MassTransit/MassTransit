namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;
    using Configuration;


    public class PropertyExpressionSagaQueryFactory<TInstance, TData, TProperty> :
        ISagaQueryFactory<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, TProperty>> _propertyExpression;
        readonly ISagaQueryPropertySelector<TData, TProperty> _selector;

        public PropertyExpressionSagaQueryFactory(Expression<Func<TInstance, TProperty>> propertyExpression,
            ISagaQueryPropertySelector<TData, TProperty> selector)
        {
            _propertyExpression = propertyExpression;
            _selector = selector;
        }

        bool ISagaQueryFactory<TInstance, TData>.TryCreateQuery(ConsumeContext<TData> context, out ISagaQuery<TInstance> query)
        {
            if (_selector.TryGetProperty(context, out var propertyValue))
            {
                Expression<Func<TInstance, bool>> filterExpression = CreateExpression(propertyValue);

                query = new SagaQuery<TInstance>(filterExpression);
                return true;
            }

            query = default;
            return false;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("property", _propertyExpression.ToString());
        }

        Expression<Func<TInstance, bool>> CreateExpression(TProperty propertyValue)
        {
            var valueExpression = Expression.Constant(propertyValue, typeof(TProperty));
            var binaryExpression = Expression.Equal(_propertyExpression.Body, valueExpression);
            Expression<Func<TInstance, bool>> lambdaExpression = Expression.Lambda<Func<TInstance, bool>>(binaryExpression, _propertyExpression.Parameters);

            return lambdaExpression;
        }
    }
}
