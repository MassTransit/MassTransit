namespace MassTransit.KafkaIntegration
{
    using System;
    using Builders;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using MassTransit.Registration;
    using Serializers;


    public class KafkaReceiveEndpointBuilder<TKey, TValue> :
        ReceiveEndpointBuilder
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly ReceiveSettings _receiveSettings;
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;
        readonly IHeadersDeserializer _headersDeserializer;

        public KafkaReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration,
            IHeadersDeserializer headersDeserializer, ReceiveSettings receiveSettings,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
            _headersDeserializer = headersDeserializer;
            _receiveSettings = receiveSettings;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public KafkaReceiveEndpointContext<TKey, TValue> CreateReceiveEndpointContext()
        {
            var context = new KafkaReceiveEndpointContext<TKey, TValue>(_busInstance, _configuration, _receiveSettings, _headersDeserializer,
                _consumerBuilderFactory);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
