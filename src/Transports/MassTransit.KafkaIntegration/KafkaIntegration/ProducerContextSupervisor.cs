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
        TransportPipeContextSupervisor<ProducerContext<TKey, TValue>>,
        IProducerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservers;
        readonly ISendPipe _sendPipe;
        readonly ISerialization _serialization;
        readonly KafkaTopicAddress _topicAddress;

        public ProducerContextSupervisor(string topicName,
            ISendPipe sendPipe, SendObservable sendObservers, IClientContextSupervisor clientContextSupervisor,
            IHostConfiguration hostConfiguration, IHeadersSerializer headersSerializer,
            Func<ProducerBuilder<TKey, TValue>> producerBuilderFactory, ISerialization serialization)
            : base(new ProducerContextFactory<TKey, TValue>(clientContextSupervisor, hostConfiguration, headersSerializer, producerBuilderFactory))
        {
            _sendObservers = sendObservers;
            _hostConfiguration = hostConfiguration;
            _serialization = serialization;
            _topicAddress = new KafkaTopicAddress(hostConfiguration.HostAddress, topicName);
            _sendPipe = sendPipe;

            clientContextSupervisor.AddSendAgent(this);
        }

        public ITopicProducer<TKey, TValue> CreateProducer()
        {
            var context = new KafkaTransportContext(_sendPipe, _hostConfiguration, _topicAddress, this, _serialization);

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
                IProducerContextSupervisor<TKey, TValue> supervisor, ISerialization serialization)
                : base(hostConfiguration, serialization)
            {
                _hostConfiguration = hostConfiguration;
                _supervisor = supervisor;
                SendPipe = sendPipe;
                TopicAddress = topicAddress;
            }

            public Uri HostAddress => _hostConfiguration.HostAddress;
            public KafkaTopicAddress TopicAddress { get; }
            public ISendPipe SendPipe { get; }

            public Task Send(IPipe<ProducerContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), _supervisor, cancellationToken);
            }

            public override string EntityName => TopicAddress.Topic;
            public override string ActivitySystem => "kafka";
        }
    }
}
