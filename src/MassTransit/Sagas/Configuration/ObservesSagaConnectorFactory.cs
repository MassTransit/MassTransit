namespace MassTransit.Configuration
{
    using System;
    using System.Linq.Expressions;
    using Middleware;
    using Saga;


    public class ObservesSagaConnectorFactory<TSaga, TMessage> :
        ISagaConnectorFactory
        where TSaga : class, ISaga, Observes<TMessage, TSaga>
        where TMessage : class
    {
        readonly ISagaMessageConnector<TSaga> _connector;

        public ObservesSagaConnectorFactory()
        {
            var policy = new AnyExistingSagaPolicy<TSaga, TMessage>();

            ISagaQueryFactory<TSaga, TMessage> queryFactory = new ExpressionSagaQueryFactory<TSaga, TMessage>(GetFilterExpression());

            var consumeFilter = new ObservesSagaMessageFilter<TSaga, TMessage>();

            _connector = new QuerySagaMessageConnector<TSaga, TMessage>(consumeFilter, policy, queryFactory);
        }

        ISagaMessageConnector<T> ISagaConnectorFactory.CreateMessageConnector<T>()
        {
            var connector = _connector as ISagaMessageConnector<T>;
            if (connector == null)
                throw new ArgumentException("The saga type did not match the connector type");

            return connector;
        }

        static Expression<Func<TSaga, TMessage, bool>> GetFilterExpression()
        {
            var instance = SagaMetadataCache<TSaga>.FactoryMethod(NewId.NextGuid());

            return instance.CorrelationExpression;
        }
    }
}
