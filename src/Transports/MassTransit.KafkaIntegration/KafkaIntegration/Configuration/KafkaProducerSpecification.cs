namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Observables;
    using Serializers;
    using Transports;


    public class KafkaProducerSpecification<TKey, TValue> :
        IKafkaProducerSpecification<TKey, TValue>,
        IKafkaProducerConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly List<Action<ISendPipeConfigurator>> _configureSend;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        readonly ProducerConfig _producerConfig;
        readonly SendObservable _sendObservers;
        readonly SerializationConfiguration _serialization;
        IHeadersSerializer _headersSerializer;
        IAsyncSerializer<TKey> _keySerializer;
        Action<string> _statisticsHandler;
        IAsyncSerializer<TValue> _valueSerializer;

        public KafkaProducerSpecification(IKafkaHostConfiguration hostConfiguration, ProducerConfig producerConfig, string topicName,
            Action<IClient, string> oAuthBearerTokenRefreshHandler, List<Action<ISendPipeConfigurator>> configureSend)
        {
            _hostConfiguration = hostConfiguration;
            _producerConfig = producerConfig;
            TopicName = topicName;
            _oAuthBearerTokenRefreshHandler = oAuthBearerTokenRefreshHandler;
            _configureSend = configureSend;
            _sendObservers = new SendObservable();
            _serialization = new SerializationConfiguration();
        }

        public int? BatchNumMessages
        {
            set => _producerConfig.BatchNumMessages = value;
        }

        public CompressionType? CompressionType
        {
            set => _producerConfig.CompressionType = value;
        }

        public int? CompressionLevel
        {
            set => _producerConfig.CompressionLevel = value;
        }

        public int? QueueBufferingBackpressureThreshold
        {
            set => _producerConfig.QueueBufferingBackpressureThreshold = value;
        }

        public TimeSpan? RetryBackoff
        {
            set => _producerConfig.RetryBackoffMs = (int?)value?.TotalMilliseconds;
        }

        public int? MessageSendMaxRetries
        {
            set => _producerConfig.MessageSendMaxRetries = value;
        }

        public TimeSpan? Linger
        {
            set => _producerConfig.LingerMs = (int?)value?.TotalMilliseconds;
        }

        public int? QueueBufferingMaxKbytes
        {
            set => _producerConfig.QueueBufferingMaxKbytes = value;
        }

        public int? QueueBufferingMaxMessages
        {
            set => _producerConfig.QueueBufferingMaxMessages = value;
        }

        public bool? EnableGaplessGuarantee
        {
            set => _producerConfig.EnableGaplessGuarantee = value;
        }

        public bool? EnableIdempotence
        {
            set => _producerConfig.EnableIdempotence = value;
        }

        public TimeSpan? TransactionTimeout
        {
            set => _producerConfig.TransactionTimeoutMs = (int?)value?.TotalMilliseconds;
        }

        public string TransactionalId
        {
            set => _producerConfig.TransactionalId = value;
        }

        public Partitioner? Partitioner
        {
            set => _producerConfig.Partitioner = value;
        }

        public TimeSpan? MessageTimeout
        {
            set => _producerConfig.MessageTimeoutMs = (int?)value?.TotalMilliseconds;
        }

        public TimeSpan? RequestTimeout
        {
            set => _producerConfig.RequestTimeoutMs = (int?)value?.TotalMilliseconds;
        }

        public string DeliveryReportFields
        {
            set => _producerConfig.DeliveryReportFields = value;
        }

        public bool? EnableDeliveryReports
        {
            set => _producerConfig.EnableDeliveryReports = value;
        }

        public bool? EnableBackgroundPoll
        {
            set => _producerConfig.EnableBackgroundPoll = value;
        }

        public void SetStatisticsHandler(Action<string> statisticsHandler)
        {
            _statisticsHandler = statisticsHandler ?? throw new ArgumentNullException(nameof(statisticsHandler));
        }

        public void SetKeySerializer(IAsyncSerializer<TKey> serializer)
        {
            _keySerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetValueSerializer(IAsyncSerializer<TValue> serializer)
        {
            _valueSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string TopicName { get; }

        public KafkaSendTransportContext<TKey, TValue> CreateSendTransportContext(IBusInstance busInstance, Action onStop = null)
        {
            ProducerBuilder<byte[], byte[]> CreateProducerBuilder()
            {
                var producerConfig = _hostConfiguration.GetProducerConfig(_producerConfig);

                ProducerBuilder<byte[], byte[]> producerBuilder = new ProducerBuilder<byte[], byte[]>(producerConfig)
                    .SetKeySerializer(Serializers.ByteArray)
                    .SetValueSerializer(Serializers.ByteArray);

                if (_oAuthBearerTokenRefreshHandler != null)
                    producerBuilder.SetOAuthBearerTokenRefreshHandler(_oAuthBearerTokenRefreshHandler);

                if (_statisticsHandler != null)
                    producerBuilder.SetStatisticsHandler((_, statistics) => _statisticsHandler(statistics));
                return producerBuilder;
            }

            var supervisor = new ProducerContextSupervisor(_hostConfiguration.ClientContextSupervisor, busInstance.HostConfiguration, CreateProducerBuilder,
                onStop);

            var transportContext = new KafkaTopicSendTransportContext<TKey, TValue>(busInstance.HostConfiguration, TopicName, supervisor, _headersSerializer,
                _keySerializer, _valueSerializer, _configureSend, _serialization.CreateSerializerCollection());
            transportContext.ConnectSendObserver(_sendObservers);

            return transportContext;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(TopicName))
                yield return this.Failure("Topic", "should not be empty");

            if (_headersSerializer == null)
                yield return this.Failure("HeadersSerializer", "should not be null");

            if (_keySerializer == null)
                yield return this.Failure("KeySerializer", "should not be null");

            if (_valueSerializer == null)
                yield return this.Failure("ValueSerializer", "should not be null");
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }
    }
}
