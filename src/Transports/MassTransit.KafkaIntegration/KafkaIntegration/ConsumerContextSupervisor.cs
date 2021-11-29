namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Serializers;
    using Transports;


    public class ConsumerContextSupervisor<TKey, TValue> :
        TransportPipeContextSupervisor<ConsumerContext<TKey, TValue>>,
        IConsumerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        public ConsumerContextSupervisor(IClientContextSupervisor clientContextSupervisor, ReceiveSettings receiveSettings,
            IHostConfiguration hostConfiguration, IHeadersDeserializer headersDeserializer, Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(new ConsumerContextFactory<TKey, TValue>(clientContextSupervisor, receiveSettings, hostConfiguration, headersDeserializer,
                consumerBuilderFactory))
        {
            clientContextSupervisor.AddConsumeAgent(this);
        }
    }
}
