namespace MassTransit.Configuration
{
    using System;
    using Middleware;
    using Saga;


    public class InitiatedBySagaConnectorFactory<TSaga, TMessage> :
        ISagaConnectorFactory
        where TSaga : class, ISaga, InitiatedBy<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly ISagaMessageConnector<TSaga> _connector;

        public InitiatedBySagaConnectorFactory()
        {
            var consumeFilter = new InitiatedBySagaMessageFilter<TSaga, TMessage>();

            ISagaFactory<TSaga, TMessage> sagaFactory = new DefaultSagaFactory<TSaga, TMessage>();

            var policy = new NewSagaPolicy<TSaga, TMessage>(sagaFactory, false);

            _connector = new CorrelatedSagaMessageConnector<TSaga, TMessage>(consumeFilter, policy, x => x.Message.CorrelationId);
        }

        ISagaMessageConnector<T> ISagaConnectorFactory.CreateMessageConnector<T>()
        {
            if (_connector is ISagaMessageConnector<T> connector)
                return connector;

            throw new ArgumentException("The saga type did not match the connector type");
        }
    }
}
