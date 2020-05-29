namespace MassTransit.Saga.Connectors
{
    using System;
    using Factories;
    using Pipeline.Filters;
    using Policies;


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
            var connector = _connector as ISagaMessageConnector<T>;
            if (connector == null)
                throw new ArgumentException("The saga type did not match the connector type");

            return connector;
        }
    }
}
