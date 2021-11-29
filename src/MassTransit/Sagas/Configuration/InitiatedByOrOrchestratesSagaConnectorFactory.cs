namespace MassTransit.Configuration
{
    using System;
    using Middleware;
    using Saga;


    public class InitiatedByOrOrchestratesSagaConnectorFactory<TSaga, TMessage> :
        ISagaConnectorFactory
        where TSaga : class, ISaga, InitiatedByOrOrchestrates<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly ISagaMessageConnector<TSaga> _connector;

        public InitiatedByOrOrchestratesSagaConnectorFactory()
        {
            var consumeFilter = new InitiatedByOrOrchestratesSagaMessageFilter<TSaga, TMessage>();

            ISagaFactory<TSaga, TMessage> sagaFactory = new DefaultSagaFactory<TSaga, TMessage>();

            var policy = new NewOrExistingSagaPolicy<TSaga, TMessage>(sagaFactory, false);

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
