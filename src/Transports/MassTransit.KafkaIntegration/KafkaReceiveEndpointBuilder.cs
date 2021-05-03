namespace MassTransit.KafkaIntegration
{
    using System;
    using Builders;
    using Configuration;
    using Configurators;
    using Confluent.Kafka;
    using MassTransit.Registration;
    using Serializers;


    public class KafkaReceiveEndpointBuilder<TKey, TValue> :
        ReceiveEndpointBuilder
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly CheckpointPipeConfiguration _checkpointPipeConfiguration;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public KafkaReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration,
            IKafkaHostConfiguration hostConfiguration, ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory, CheckpointPipeConfiguration checkpointPipeConfiguration)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _consumerBuilderFactory = consumerBuilderFactory;
            _checkpointPipeConfiguration = checkpointPipeConfiguration;
        }

        public IKafkaReceiveEndpointContext<TKey, TValue> CreateReceiveEndpointContext()
        {
            var context = new KafkaReceiveEndpointContext<TKey, TValue>(_busInstance, _configuration, _hostConfiguration, _receiveSettings,
                _headersDeserializer, _consumerBuilderFactory, _checkpointPipeConfiguration);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
