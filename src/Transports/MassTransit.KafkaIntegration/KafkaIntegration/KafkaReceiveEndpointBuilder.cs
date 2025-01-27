namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Serializers;
    using Transports;


    public class KafkaReceiveEndpointBuilder<TKey, TValue> :
        ReceiveEndpointBuilder
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly string _groupId;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly ReceiveSettings _receiveSetting;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IDeserializer<TKey> _keyDeserializer;
        readonly IDeserializer<TValue> _valueDeserializer;
        readonly ConsumerBuilderFactory _consumerBuilderFactory;

        public KafkaReceiveEndpointBuilder(IBusInstance busInstance, IKafkaHostConfiguration hostConfiguration,
            string groupId, IReceiveEndpointConfiguration endpointConfiguration, ReceiveSettings receiveSetting,
            IHeadersDeserializer headersDeserializer, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer,
            ConsumerBuilderFactory consumerBuilderFactory)
            : base(endpointConfiguration)
        {
            _busInstance = busInstance;
            _hostConfiguration = hostConfiguration;
            _groupId = groupId;
            _endpointConfiguration = endpointConfiguration;
            _receiveSetting = receiveSetting;
            _headersDeserializer = headersDeserializer;
            _keyDeserializer = keyDeserializer;
            _valueDeserializer = valueDeserializer;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public KafkaReceiveEndpointContext<TKey, TValue> CreateReceiveEndpointContext()
        {
            var context = new TopicKafkaReceiveEndpointContext<TKey, TValue>(_busInstance, _hostConfiguration, _groupId, _endpointConfiguration,
                _receiveSetting, _headersDeserializer, _keyDeserializer, _valueDeserializer, _consumerBuilderFactory);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.Topology);
            context.AddOrUpdatePayload(() => _receiveSetting, _ => _receiveSetting);

            return context;
        }
    }
}
