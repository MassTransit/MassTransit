namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Observables;
    using Serializers;
    using Transports;


    public class ProducerContextSupervisor<TKey, TValue> :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly IHeadersSerializer _headersSerializer;
        readonly IAsyncSerializer<TKey> _keySerializer;
        readonly IAsyncSerializer<TValue> _valueSerializer;
        readonly SendObservable _sendObservers;
        readonly ISendPipe _sendPipe;
        readonly ISerialization _serialization;
        readonly KafkaTopicAddress _topicAddress;

        public ProducerContextSupervisor(string topicName,
            ISendPipe sendPipe, SendObservable sendObservers, IClientContextSupervisor clientContextSupervisor,
            IHostConfiguration hostConfiguration, IHeadersSerializer headersSerializer,
            IAsyncSerializer<TKey> keySerializer, IAsyncSerializer<TValue> valueSerializer,
            Func<ProducerBuilder<byte[], byte[]>> producerBuilderFactory, ISerialization serialization)
            : base(new ProducerContextFactory(clientContextSupervisor, hostConfiguration, producerBuilderFactory))
        {
            _sendObservers = sendObservers;
            _hostConfiguration = hostConfiguration;
            _headersSerializer = headersSerializer;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
            _serialization = serialization;
            _topicAddress = new KafkaTopicAddress(hostConfiguration.HostAddress, topicName);
            _sendPipe = sendPipe;

            clientContextSupervisor.AddSendAgent(this);
        }

        public ITopicProducer<TKey, TValue> CreateProducer()
        {
            var context = new KafkaTransportContext(_sendPipe, _hostConfiguration, _topicAddress, this, _headersSerializer, _keySerializer, _valueSerializer,
                _serialization);

            if (_sendObservers.Count > 0)
                context.ConnectSendObserver(_sendObservers);

            return new TopicProducer<TKey, TValue>(context);
        }


        class KafkaTransportContext :
            BaseSendTransportContext,
            KafkaSendTransportContext<TKey, TValue>
        {
            readonly IHostConfiguration _hostConfiguration;
            readonly IProducerContextSupervisor<TKey, TValue> _supervisor;

            public KafkaTransportContext(ISendPipe sendPipe, IHostConfiguration hostConfiguration, KafkaTopicAddress topicAddress,
                IProducerContextSupervisor<TKey, TValue> supervisor, IHeadersSerializer headersSerializer, IAsyncSerializer<TKey> keySerializer,
                IAsyncSerializer<TValue> valueSerializer,
                ISerialization serialization)
                : base(hostConfiguration, serialization)
            {
                _hostConfiguration = hostConfiguration;
                _supervisor = supervisor;
                SendPipe = sendPipe;
                TopicAddress = topicAddress;
                HeadersSerializer = headersSerializer;
                KeySerializer = keySerializer;
                ValueSerializer = valueSerializer;
            }

            public Uri HostAddress => _hostConfiguration.HostAddress;
            public KafkaTopicAddress TopicAddress { get; }
            public ISendPipe SendPipe { get; }
            public IHeadersSerializer HeadersSerializer { get; }

            public IAsyncSerializer<TValue> ValueSerializer { get; }
            public IAsyncSerializer<TKey> KeySerializer { get; }

            public Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
            }

            public override string EntityName => TopicAddress.Topic;
            public override string ActivitySystem => "kafka";

            public override Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            {
                throw new NotImplementedByDesignException("Kafka is a producer, not an outbox compatible transport");
            }
        }
    }
}
