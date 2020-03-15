namespace MassTransit.Definition
{
    using System;
    using Saga;


    /// <summary>
    /// A saga message definition defines the configuration for a saga message, and may also be used to configure the
    /// saga repository, create indices, etc.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMessageDefinition<TSaga, TMessage> :
        ISagaMessageDefinition<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        ISagaDefinition<TSaga> _sagaDefinition;

        int? _concurrentMessageLimit;

        public SagaMessageDefinition(ISagaDefinition<TSaga> sagaDefinition)
        {
            _sagaDefinition = sagaDefinition;
        }

        /// <summary>
        /// Set the concurrent message limit for the saga, which limits how many saga instances are able to concurrently
        /// consume messages.
        /// </summary>
        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit;
            protected set => _concurrentMessageLimit = value;
        }

        public Type SagaType => typeof(TSaga);

        public Type MessageType => typeof(TMessage);

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
        {
        }

        void ISagaMessageDefinition<TSaga, TMessage>.Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaMessageConfigurator<TSaga, TMessage>
            sagaConfigurator)
        {
            // if (_concurrentMessageLimit.HasValue)
            //     sagaConfigurator.UseConcurrentMessageLimit(_concurrentMessageLimit.Value);
        }
    }
}
