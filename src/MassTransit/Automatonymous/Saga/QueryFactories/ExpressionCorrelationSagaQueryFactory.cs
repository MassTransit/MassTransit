namespace Automatonymous.Saga.QueryFactories
{
    using System;
    using System.Linq.Expressions;
    using CorrelationConfigurators;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class ExpressionCorrelationSagaQueryFactory<TInstance, TData> :
        ISagaQueryFactory<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, ConsumeContext<TData>, bool>> _correlationExpression;

        public ExpressionCorrelationSagaQueryFactory(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            _correlationExpression = correlationExpression;
        }

        bool ISagaQueryFactory<TInstance, TData>.TryCreateQuery(ConsumeContext<TData> context, out ISagaQuery<TInstance> query)
        {
            Expression<Func<TInstance, bool>> filter = new EventCorrelationExpressionConverter<TInstance, TData>(context)
                .Convert(_correlationExpression);

            query = new SagaQuery<TInstance>(filter);
            return true;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("expression", _correlationExpression.ToString());
        }
    }
}
