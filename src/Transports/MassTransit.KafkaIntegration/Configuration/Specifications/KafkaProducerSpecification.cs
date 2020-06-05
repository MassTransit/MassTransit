namespace MassTransit.KafkaIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Serializers;
    using Transport;


    public class KafkaProducerSpecification<TKey, TValue> :
        IKafkaProducerSpecification,
        IKafkaProducerConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly ProducerConfig _producerConfig;
        readonly string _topicName;
        readonly SendObservable _sendObservers;
        IHeadersSerializer _headersSerializer;
        ISerializer<TKey> _keySerializer;
        ISerializer<TValue> _valueSerializer;

        public KafkaProducerSpecification(ProducerConfig producerConfig, string topicName, IHeadersSerializer headersSerializer)
        {
            _producerConfig = producerConfig;
            _topicName = topicName;
            _headersSerializer = headersSerializer;
            _sendObservers = new SendObservable();

            SetValueSerializer(new MassTransitSerializer<TValue>());
            SetHeadersSerializer(headersSerializer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
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
            set => _producerConfig.RetryBackoffMs = value?.Milliseconds;
        }

        public int? MessageSendMaxRetries
        {
            set => _producerConfig.MessageSendMaxRetries = value;
        }

        public TimeSpan? Linger
        {
            set => _producerConfig.LingerMs = value?.Milliseconds;
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
            set => _producerConfig.TransactionTimeoutMs = value?.Milliseconds;
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
            set => _producerConfig.MessageTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? RequestTimeout
        {
            set => _producerConfig.RequestTimeoutMs = value?.Milliseconds;
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

        public void SetKeySerializer(ISerializer<TKey> serializer)
        {
            _keySerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetValueSerializer(ISerializer<TValue> serializer)
        {
            _valueSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IKafkaProducerFactory CreateProducerFactory(IBusInstance busInstance)
        {
            var logContext = busInstance.HostConfiguration.SendLogContext;

            ProducerBuilder<TKey, TValue> producerBuilder = new ProducerBuilder<TKey, TValue>(_producerConfig)
                .SetErrorHandler((c, error) => logContext?.Error?.Log("Consumer error ({code}): {reason} on {topic}", error.Code, error.Reason, _topicName))
                .SetLogHandler((c, message) => logContext?.Info?.Log(message.Message));

            if (_keySerializer != null)
                producerBuilder.SetKeySerializer(_keySerializer);
            if (_valueSerializer != null)
                producerBuilder.SetValueSerializer(_valueSerializer);
            busInstance.BusControl.Topology.SendTopology.GetMessageTopology<TValue>();

            return new KafkaProducerFactory<TKey, TValue>(_topicName, producerBuilder.Build(), busInstance, _headersSerializer, _sendObservers);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_producerConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");
        }
    }
}
