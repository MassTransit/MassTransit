namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using Confluent.Kafka;
    using Context;
    using GreenPipes.Agents;
    using Serializers;
    using Transport;
    using Transports;


    public class KafkaConsumerContextSupervisor<TKey, TValue> :
        TransportPipeContextSupervisor<IKafkaConsumerContext<TKey, TValue>>,
        IKafkaConsumerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        public KafkaConsumerContextSupervisor(IAgent agent, ReceiveSettings receiveSettings, ILogContext logContext, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(agent, new KafkaConsumerContextFactory<TKey, TValue>(receiveSettings, logContext, headersDeserializer, consumerBuilderFactory))
        {
        }
    }
}
