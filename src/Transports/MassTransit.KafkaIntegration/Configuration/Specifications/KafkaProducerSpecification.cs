namespace MassTransit.KafkaIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline;
    using Pipeline.Observables;
    using Serializers;
    using Transport;


    public class KafkaProducerSpecification<TKey, TValue> :
        IKafkaProducerSpecification,
        IKafkaProducerConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly ProducerConfig _producerConfig;
        readonly SendObservable _sendObservers;
        readonly string _topicName;
        Action<ISendPipeConfigurator> _configureSend;
        IHeadersSerializer _headersSerializer;
        ISerializer<TKey> _keySerializer;
        ISerializer<TValue> _valueSerializer;

        public KafkaProducerSpecification(ProducerConfig producerConfig, string topicName, IHeadersSerializer headersSerializer)
        {
            _producerConfig = producerConfig;
            _topicName = topicName;
            _headersSerializer = headersSerializer;
            _sendObservers = new SendObservable();

            SetKeySerializer(new MassTransitJsonSerializer<TKey>());
            SetValueSerializer(new MassTransitJsonSerializer<TValue>());
            SetHeadersSerializer(headersSerializer);
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
            set => _producerConfig.RetryBackoffMs = Convert.ToInt32(value?.TotalMilliseconds);
        }

        public int? MessageSendMaxRetries
        {
            set => _producerConfig.MessageSendMaxRetries = value;
        }

        public TimeSpan? Linger
        {
            set => _producerConfig.LingerMs = Convert.ToInt32(value?.TotalMilliseconds);
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
            set => _producerConfig.TransactionTimeoutMs = Convert.ToInt32(value?.TotalMilliseconds);
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
            set => _producerConfig.MessageTimeoutMs = Convert.ToInt32(value?.TotalMilliseconds);
        }

        public TimeSpan? RequestTimeout
        {
            set => _producerConfig.RequestTimeoutMs = Convert.ToInt32(value?.TotalMilliseconds);
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

            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);

            ProducerBuilder<TKey, TValue> producerBuilder = new ProducerBuilder<TKey, TValue>(_producerConfig)
                .SetErrorHandler((c, error) => logContext?.Error?.Log("Consumer error ({code}): {reason} on {topic}", error.Code, error.Reason, _topicName))
                .SetLogHandler((c, message) => logContext?.Info?.Log(message.Message));

            if (_keySerializer != null)
                producerBuilder.SetKeySerializer(_keySerializer);
            if (_valueSerializer != null)
                producerBuilder.SetValueSerializer(_valueSerializer);

            var context = new KafkaProducerContext(producerBuilder.Build(), busInstance.HostConfiguration, sendConfiguration, _sendObservers,
                _headersSerializer);

            return new KafkaProducerFactory<TKey, TValue>(new KafkaTopicAddress(busInstance.HostConfiguration.HostAddress, _topicName), context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_producerConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }


        class KafkaProducerContext :
            IKafkaProducerContext<TKey, TValue>
        {
            readonly IHostConfiguration _hostConfiguration;
            readonly IProducer<TKey, TValue> _producer;
            readonly ISendPipe _sendPipe;

            public KafkaProducerContext(IProducer<TKey, TValue> producer, IHostConfiguration hostConfiguration, ISendPipeConfiguration sendConfiguration,
                SendObservable sendObservers, IHeadersSerializer headersSerializer)
            {
                _producer = producer;
                _hostConfiguration = hostConfiguration;
                _sendPipe = sendConfiguration.CreatePipe();
                SendObservers = sendObservers;
                HeadersSerializer = headersSerializer;
            }

            public Uri HostAddress => _hostConfiguration.HostAddress;
            public ILogContext LogContext => _hostConfiguration.SendLogContext;
            public SendObservable SendObservers { get; }

            public IHeadersSerializer HeadersSerializer { get; }

            public Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
            {
                return _producer.ProduceAsync(partition, message, cancellationToken);
            }

            public void Dispose()
            {
                var timeout = TimeSpan.FromSeconds(30);
                _producer.Flush(timeout);
                _producer.Dispose();
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                return _sendPipe.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                _sendPipe.Probe(context);
            }
        }
    }
}
