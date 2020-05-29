namespace MassTransit.Saga.QueryFactories
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes;


    /// <summary>
    /// Creates a saga query using the specified filter expression
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ExpressionSagaQueryFactory<TSaga, TMessage> :
        ISagaQueryFactory<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;

        public ExpressionSagaQueryFactory(Expression<Func<TSaga, TMessage, bool>> filterExpression)
        {
            _filterExpression = filterExpression;
        }

        ISagaQuery<TSaga> ISagaQueryFactory<TSaga, TMessage>.CreateQuery(ConsumeContext<TMessage> context)
        {
            Expression<Func<TSaga, bool>> expression = new SagaFilterExpressionConverter<TSaga, TMessage>(context.Message)
                .Convert(_filterExpression);

            return new SagaQuery<TSaga>(expression);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("expression", _filterExpression.ToString());
        }
    }
}
