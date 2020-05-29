namespace Automatonymous.Saga.QueryFactories
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class PropertyExpressionSagaQueryFactory<TInstance, TData, TProperty> :
        ISagaQueryFactory<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, TProperty>> _propertyExpression;
        readonly Func<ConsumeContext<TData>, TProperty> _selector;

        public PropertyExpressionSagaQueryFactory(Expression<Func<TInstance, TProperty>> propertyExpression, Func<ConsumeContext<TData>, TProperty> selector)
        {
            _propertyExpression = propertyExpression;
            _selector = selector;
        }

        ISagaQuery<TInstance> ISagaQueryFactory<TInstance, TData>.CreateQuery(ConsumeContext<TData> context)
        {
            var propertyValue = _selector(context);

            Expression<Func<TInstance, bool>> filterExpression = CreateExpression(propertyValue);

            return new SagaQuery<TInstance>(filterExpression);
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
